package com.gameplus.mahjong;

import android.app.Application;
import android.content.Context;
import android.support.multidex.MultiDex;

import io.fabric.unity.android.FabricInitializer;

/**
 * Created Jansen on 2017/6/28.
 */
public class InitApplication extends Application{
    @Override
    public void onCreate() {
        super.onCreate();
        FabricInitializer.initializeFabric(this, FabricInitializer.Caller.Android);
    }

    @Override
    protected void attachBaseContext(Context base) {
        super.attachBaseContext(base);
        MultiDex.install(this);
    }
}
