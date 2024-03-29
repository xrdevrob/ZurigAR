﻿/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/         
* 
*****************************************************************************/

namespace NRKernal
{
    using System;
    using UnityEngine;

    public class NRDefaultPhoneScreenProvider : NRPhoneScreenProviderBase
    {
        private static AndroidJavaObject m_VirtualDisplayFragment;
        public class AndroidSystemButtonDataProxy : AndroidJavaProxy, ISystemButtonDataProxy
        {
            private NRPhoneScreenProviderBase m_Provider;

            public AndroidSystemButtonDataProxy(NRPhoneScreenProviderBase provider) : base("ai.nreal.virtualcontroller.ISystemButtonDataReceiver")
            {
                this.m_Provider = provider;
            }

            public void OnUpdate(AndroidJavaObject data)
            {
                SystemButtonState state = new SystemButtonState();
#if UNITY_2019_1_OR_NEWER
                sbyte[] sbuffer = data.Call<sbyte[]>("getRawData");
                byte[] bytes = new byte[sbuffer.Length];
                Buffer.BlockCopy(sbuffer, 0, bytes, 0, bytes.Length);
#else
                byte[] bytes = data.Call<byte[]>("getRawData");
#endif
                state.DeSerialize(bytes);
                m_Provider.OnSystemButtonDataChanged(state);
            }
        }

        public override void RegistFragment(AndroidJavaObject unityActivity, ISystemButtonDataProxy proxy)
        {
            NRDebugger.Info("[VirtualController] RegistFragment...");
            var VirtualDisplayFragment = new AndroidJavaClass("ai.nreal.virtualcontroller.VirtualControllerFragment");
            m_VirtualDisplayFragment = VirtualDisplayFragment.CallStatic<AndroidJavaObject>("RegistFragment", unityActivity, proxy);
        }

        public static void RegistDebugInfoProxy(AndroidJavaProxy proxy)
        {
            if (m_VirtualDisplayFragment != null)
            {
                m_VirtualDisplayFragment.Call("setDebugInfoProvider", proxy);
            }
        }

        public override ISystemButtonDataProxy CreateAndroidDataProxy()
        {
            return new AndroidSystemButtonDataProxy(this);
        }
    }
}
