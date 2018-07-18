#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// before receipt validation will compile in this sample.
//#define RECEIPT_VALIDATION
#endif

using Assets.GamePlus.bean;
using Assets.GamePlus.listner;
using Assets.Script.gameplus.define;
using Assets.Script.utils;
using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.FirebaseController;
using UnityEngine;
using UnityEngine.Purchasing;

public class InAppPurchaseSup : MonoBehaviour,IStoreListener
{
    private IStoreController m_StoreController;
    private IAppleExtensions m_AppleExtensions;
	#if RECEIPT_VALIDATION
	private CrossPlatformValidator validator;
	#endif
    private bool m_IsGooglePlayStoreSelected;
    public bool m_PurchaseInProgress = false;
    public PurchaseListner mlistener;
    private Hashtable purchaseItems;
    private string current_id = "";
    private string purchase_location = "";
    public static string Transactionid="";
    public void SetPurchaseListner(PurchaseListner mlistener)
    {
        this.mlistener = mlistener;
    }
    //初始化内购
    public void Awake()
    {
        var module = StandardPurchasingModule.Instance();
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
        purchaseItems=new Hashtable();
        //添加商品,ios和android ID相同
        PurController.InitPurchase(ref builder, ref purchaseItems);
        //ios和android ID不相同
        //builder.AddProduct(AppConfig.PURCHASE_COIN_200, ProductType.NonConsumable, new IDs
        //{
        //    {"coin_200_ios", AppleAppStore.Name},
        //    {"coin_200_android", GooglePlay.Name},
        //});
        m_IsGooglePlayStoreSelected = Application.platform == RuntimePlatform.Android && module.androidStore == AndroidStore.GooglePlay;
		#if RECEIPT_VALIDATION
		    validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.bundleIdentifier);
		#endif
        UnityPurchasing.Initialize(this, builder);
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 购买商品
    /// </summary>
    /// <param name="productID">AppConfig中配置好的商品ID</param>
    public void PurchaseProduct(string productID,string location)
    {
        if (m_PurchaseInProgress == true)
        {
            Debug.Log("Please wait, purchasing ...");
            return;
        }

        if (m_StoreController != null)
        {
            // Fetch the currency Product reference from Unity Purchasing
            Product product = m_StoreController.products.WithID(productID);
            if (product != null && product.availableToPurchase)
            {
                m_PurchaseInProgress = true;
                AdsUtils.setInterStutas(false);
                current_id = productID;
                purchase_location = location;

                Dictionary<string, object> desc = new Dictionary<string, object>();
                desc.Add("item", current_id);
                desc.Add("status", "start purchase");
                AnalysisSup.fabricLog(EventName.PURCHASE, desc);

                m_StoreController.InitiatePurchase(product);
            }
            else {
                Debug.Log("product is null or not available ...");
            }
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Billing failed to initialize!");
        switch (error)
        {
            case InitializationFailureReason.AppNotKnown:
                Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                break;
            case InitializationFailureReason.PurchasingUnavailable:
                // Ask the user if billing is disabled in device settings.
                Debug.Log("Billing disabled!");
                break;
            case InitializationFailureReason.NoProductsAvailable:
                // Developer configuration error; check product metadata.
                Debug.Log("No products available for purchase!");
                break;
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreController = controller;
        m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
        m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        //弹出原生提示框提示失败原因
        Debug.Log("OnPurchaseFailed =====>" + p);

        Dictionary<string, object> desc = new Dictionary<string, object>();
        desc.Add("item", current_id);
        desc.Add("status", "fail purchase");
        AnalysisSup.fabricLog(EventName.PURCHASE, desc);

        m_PurchaseInProgress = false;
        if (mlistener != null) {
            mlistener.PurchaseResult(false, "");
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.Log("Purchase OK: " + e.purchasedProduct.definition.id);
        Debug.Log("Receipt: " + e.purchasedProduct.receipt);
        Transactionid = e.purchasedProduct.transactionID;
        bool validPurchase = true;
        #if RECEIPT_VALIDATION
        // 校验购买成功的商品
		// Local validation is available for GooglePlay and Apple stores
		    if (m_IsGooglePlayStoreSelected ||
			    Application.platform == RuntimePlatform.IPhonePlayer ||
			    Application.platform == RuntimePlatform.OSXPlayer ||
			    Application.platform == RuntimePlatform.tvOS) {
			    try {
				    var result = validator.Validate(e.purchasedProduct.receipt);
				    Debug.Log("Receipt is valid. Contents:");
				    foreach (IPurchaseReceipt productReceipt in result) {
					    Debug.Log(productReceipt.productID);
					    Debug.Log(productReceipt.purchaseDate);
					    Debug.Log(productReceipt.transactionID);

					    GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
					    if (null != google) {
						    Debug.Log(google.purchaseState);
						    Debug.Log(google.purchaseToken);
					    }

					    AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
					    if (null != apple) {
						    Debug.Log(apple.originalTransactionIdentifier);
						    Debug.Log(apple.subscriptionExpirationDate);
						    Debug.Log(apple.cancellationDate);
						    Debug.Log(apple.quantity);
					    }
				    }
			    } catch (IAPSecurityException) {
				    Debug.Log("Invalid receipt, not unlocking content");
                    validPurchase = false;
			    }
		    }
		#endif

            if (mlistener != null)
            {
                if (validPurchase)
                {
                    //校验成功
                    mlistener.PurchaseResult(true, e.purchasedProduct.definition.id);
                    AnalysisSup.logPurchase((PurchaseItem)purchaseItems[e.purchasedProduct.definition.id], "purchase verify success", true, purchase_location);
                }
                else
                {
                    //校验失败，弹出框提示
                    AnalysisSup.logPurchase((PurchaseItem)purchaseItems[e.purchasedProduct.definition.id], "purchase verify fail", false, purchase_location);
                    mlistener.PurchaseResult(false,"");
                }
            }
        m_PurchaseInProgress = false;
        return PurchaseProcessingResult.Complete; 
    }

    /// <summary>
    /// iOS Specific.
    /// This is called as part of Apple's 'Ask to buy' functionality,
    /// when a purchase is requested by a minor and referred to a parent
    /// for approval.
    /// 
    /// When the purchase is approved or rejected, the normal purchase events
    /// will fire.
    /// </summary>
    /// <param name="item">Item.</param>
    private void OnDeferred(Product item)
    {
        Debug.Log("Purchase deferred: " + item.definition.id);
    }

    /// <summary>
    /// This will be called after a call to IAppleExtensions.RestoreTransactions().
    /// </summary>
    private void OnTransactionsRestored(bool success)
    {
        Debug.Log("Transactions restored.");
    }

    /// <summary>
    /// This will be called after a call to IAppleExtensions.RestoreTransactions().
    /// </summary>
    public void RestoreAppPurchase()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.tvOS)
        {
            m_AppleExtensions.RestoreTransactions(OnTransactionsRestored);
        }
        else
        {
            Debug.LogWarning(Application.platform.ToString() + " is not a supported platform for the Codeless IAP restore button");
        }
    }
}
