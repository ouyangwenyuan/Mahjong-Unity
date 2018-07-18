﻿// <copyright file="NativeQuestClient.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>

#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))

namespace GooglePlayGames.Native
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using GooglePlayGames.BasicApi;
    using GooglePlayGames.OurUtils;
    using GooglePlayGames.BasicApi.Quests;
    using GooglePlayGames.Native.PInvoke;
using Types = GooglePlayGames.Native.Cwrapper.Types;
    using Status = GooglePlayGames.Native.Cwrapper.CommonErrorStatus;

    internal class NativeQuestClient : IQuestsClient
    {

        private readonly QuestManager mManager;

        internal NativeQuestClient(QuestManager manager)
        {
            this.mManager = Misc.CheckNotNull(manager);
        }

        public void Fetch(DataSource source, string questId, Action<ResponseStatus, IQuest> callback)
        {
            Misc.CheckNotNull(questId);
            Misc.CheckNotNull(callback);
            callback = CallbackUtils.ToOnGameThread(callback);
            mManager.Fetch(ConversionUtils.AsDataSource(source), questId,
                response =>
                {
                    var status = ConversionUtils.ConvertResponseStatus(response.ResponseStatus());

                    if (!response.RequestSucceeded())
                    {
                        callback(status, null);
                    }
                    else
                    {
                        callback(status, response.Data());
                    }
                });
        }

        public void FetchMatchingState(DataSource source, QuestFetchFlags flags,
                                   Action<ResponseStatus, List<IQuest>> callback)
        {
            Misc.CheckNotNull(callback);
            callback = CallbackUtils.ToOnGameThread(callback);

            mManager.FetchList(ConversionUtils.AsDataSource(source), (int)flags,
                response =>
                {
                    var status = ConversionUtils.ConvertResponseStatus(response.ResponseStatus());

                    if (!response.RequestSucceeded())
                    {
                        callback(status, null);
                    }
                    else
                    {
                        callback(status, response.Data().Cast<IQuest>().ToList());
                    }
                });
        }

        public void ShowAllQuestsUI(Action<QuestUiResult, IQuest, IQuestMilestone> callback)
        {
            Misc.CheckNotNull(callback);
            callback = CallbackUtils.ToOnGameThread<QuestUiResult, IQuest, IQuestMilestone>(callback);
            mManager.ShowAllQuestUI(FromQuestUICallback(callback));
        }

        public void ShowSpecificQuestUI(IQuest quest,
                                    Action<QuestUiResult, IQuest, IQuestMilestone> callback)
        {
            Misc.CheckNotNull(quest);
            Misc.CheckNotNull(callback);
            callback = CallbackUtils.ToOnGameThread<QuestUiResult, IQuest, IQuestMilestone>(callback);

            var convertedQuest = quest as NativeQuest;

            if (convertedQuest == null)
            {
                Logger.e("Encountered quest that was not generated by this IQuestClient");
                callback(QuestUiResult.BadInput, null, null);
                return;
            }

            mManager.ShowQuestUI(convertedQuest, FromQuestUICallback(callback));
        }

        private static QuestUiResult UiErrorToQuestUiResult(Status.UIStatus status)
        {
            switch (status)
            {
                case Status.UIStatus.ERROR_INTERNAL:
                    return QuestUiResult.InternalError;
                case Status.UIStatus.ERROR_NOT_AUTHORIZED:
                    return QuestUiResult.NotAuthorized;
                case Status.UIStatus.ERROR_CANCELED:
                    return QuestUiResult.UserCanceled;
                case Status.UIStatus.ERROR_VERSION_UPDATE_REQUIRED:
                    return QuestUiResult.VersionUpdateRequired;
                case Status.UIStatus.ERROR_TIMEOUT:
                    return QuestUiResult.Timeout;
                case Status.UIStatus.ERROR_UI_BUSY:
                    return QuestUiResult.UiBusy;
                default:
                    Logger.e("Unknown error status: " + status);
                    return QuestUiResult.InternalError;
            }
        }

        private static Action<QuestManager.QuestUIResponse> FromQuestUICallback(
            Action<QuestUiResult, IQuest, IQuestMilestone> callback)
        {
            return response =>
            {
                if (!response.RequestSucceeded())
                {
                    callback(UiErrorToQuestUiResult(response.RequestStatus()), null, null);
                    return;
                }

                var acceptedQuest = response.AcceptedQuest();
                var milestone = response.MilestoneToClaim();

                // If the request succeeded, the user either selected a quest to accept, or a
                // milestone to claim. Figure out which one this is.
                if (acceptedQuest != null)
                {
                    callback(QuestUiResult.UserRequestsQuestAcceptance, acceptedQuest, null);
                    milestone.Dispose();
                }
                else if (milestone != null)
                {
                    callback(QuestUiResult.UserRequestsMilestoneClaiming, null,
                        response.MilestoneToClaim());
                    acceptedQuest.Dispose();
                }
                else
                {
                    Logger.e("Quest UI succeeded without a quest acceptance or milestone claim.");
                    acceptedQuest.Dispose();
                    milestone.Dispose();
                    callback(QuestUiResult.InternalError, null, null);
                }
            };
        }

        public void Accept(IQuest quest, Action<QuestAcceptStatus, IQuest> callback)
        {
            Misc.CheckNotNull(quest);
            Misc.CheckNotNull(callback);
            callback = CallbackUtils.ToOnGameThread<QuestAcceptStatus, IQuest>(callback);

            var convertedQuest = quest as NativeQuest;

            if (convertedQuest == null)
            {
                Logger.e("Encountered quest that was not generated by this IQuestClient");
                callback(QuestAcceptStatus.BadInput, null);
                return;
            }

            mManager.Accept(convertedQuest, response =>
                {
                    if (response.RequestSucceeded())
                    {
                        callback(QuestAcceptStatus.Success, response.AcceptedQuest());
                    }
                    else
                    {
                        callback(FromAcceptStatus(response.ResponseStatus()), null);
                    }
                });
        }

        private static QuestAcceptStatus FromAcceptStatus(Status.QuestAcceptStatus status)
        {
            switch (status)
            {
                case Status.QuestAcceptStatus.ERROR_INTERNAL:
                    return QuestAcceptStatus.InternalError;
                case Status.QuestAcceptStatus.ERROR_NOT_AUTHORIZED:
                    return QuestAcceptStatus.NotAuthorized;
                case Status.QuestAcceptStatus.ERROR_QUEST_NOT_STARTED:
                    return QuestAcceptStatus.QuestNotStarted;
                case Status.QuestAcceptStatus.ERROR_QUEST_NO_LONGER_AVAILABLE:
                    return QuestAcceptStatus.QuestNoLongerAvailable;
                case Status.QuestAcceptStatus.ERROR_TIMEOUT:
                    return QuestAcceptStatus.Timeout;
                case Status.QuestAcceptStatus.VALID:
                    return QuestAcceptStatus.Success;
                default:
                    Logger.e("Encountered unknown status: " + status);
                    return QuestAcceptStatus.InternalError;
            }
        }

        public void ClaimMilestone(IQuestMilestone milestone,
                               Action<QuestClaimMilestoneStatus, IQuest, IQuestMilestone> callback)
        {
            Misc.CheckNotNull(milestone);
            Misc.CheckNotNull(callback);
            callback = CallbackUtils.ToOnGameThread<QuestClaimMilestoneStatus, IQuest, IQuestMilestone>(
                callback);

            var convertedMilestone = milestone as NativeQuestMilestone;

            if (convertedMilestone == null)
            {
                Logger.e("Encountered milestone that was not generated by this IQuestClient");
                callback(QuestClaimMilestoneStatus.BadInput, null, null);
                return;
            }

            mManager.ClaimMilestone(convertedMilestone, response =>
                {
                    if (response.RequestSucceeded())
                    {
                        callback(QuestClaimMilestoneStatus.Success, response.Quest(),
                            response.ClaimedMilestone());
                    }
                    else
                    {
                        callback(FromClaimStatus(response.ResponseStatus()), null, null);
                    }
                });
        }

        private static QuestClaimMilestoneStatus FromClaimStatus(
            Status.QuestClaimMilestoneStatus status)
        {
            switch (status)
            {
                case Status.QuestClaimMilestoneStatus.VALID:
                    return QuestClaimMilestoneStatus.Success;
                case Status.QuestClaimMilestoneStatus.ERROR_INTERNAL:
                    return QuestClaimMilestoneStatus.InternalError;
                case Status.QuestClaimMilestoneStatus.ERROR_MILESTONE_ALREADY_CLAIMED:
                    return QuestClaimMilestoneStatus.MilestoneAlreadyClaimed;
                case Status.QuestClaimMilestoneStatus.ERROR_MILESTONE_CLAIM_FAILED:
                    return QuestClaimMilestoneStatus.MilestoneClaimFailed;
                case Status.QuestClaimMilestoneStatus.ERROR_NOT_AUTHORIZED:
                    return QuestClaimMilestoneStatus.NotAuthorized;
                case Status.QuestClaimMilestoneStatus.ERROR_TIMEOUT:
                    return QuestClaimMilestoneStatus.Timeout;
                default:
                    Logger.e("Encountered unknown status: " + status);
                    return QuestClaimMilestoneStatus.InternalError;
            }
        }
    }
}
#endif // #if (UNITY_ANDROID || UNITY_IPHONE)
