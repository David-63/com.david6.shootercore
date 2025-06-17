using System;
using UnityEngine;

namespace David6.ShooterFramework
{
    /// <summary>
    /// 자체 로그 클래스
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// 이벤트나 상황이 발생
        /// </summary>
        public static void WhatHappend() { Internal_Log("뭔가 있는데", LogType.Log); }
        /// <summary>
        /// 뭐냐면..
        /// </summary>
        /// <param name="toPrint"></param>
        public static void WhatHappend(object toPrint)
        {
            if (toPrint == null) toPrint = "없어?";

            string message = "뭐냐면..: " + toPrint;
            Internal_Log(message, LogType.Log);
        }
        /// <summary>
        /// 사소한 찐빠가 있음
        /// </summary>
        public static void AttentionPlease() { Internal_Log("옐로카드", LogType.Warning); }
        /// <summary>
        /// 이거 경고임.
        /// </summary>
        /// <param name="warning"></param>
        public static void AttentionPlease(object warning)
        {
            // Null check.
            if (warning == null) warning = "아무튼";

            string message = "이거 경고임: " + warning;
            Internal_Log(message, LogType.Warning);
        }
        /// <summary>
        /// 문제가 있어요
        /// </summary>
        public static void ErrorAlert() { Internal_Log("오 쉣", LogType.Error); }
        /// <summary>
        /// 문제가 생겼는데.
        /// </summary>
        /// <param name="error"></param>
        public static void ErrorAlert(object error)
        {
            // Null check.
            if (error == null) error = "뭔지 모르겠음";

            string message = "문제가 생겼는데: " + error;
            Internal_Log(message, LogType.Error);
        }
        /// <summary>
        /// 뭔가 예외사항 발생
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="context"></param>
        public static void OopsException(Exception exception, UnityEngine.Object context = default(UnityEngine.Object)) 
        { 
            Debug.LogException(exception, context); 
        }

        private static void Internal_Log(string message, LogType type)
        {
            // Null case.
            if (message == " ")
                message = "Null";

            switch (type)
            {
                case LogType.Log:
                    Debug.Log(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                case LogType.Assert:
                    break;
                case LogType.Exception:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(type.GetType().FullName, type, null);
            }
        }

    }

    
}

namespace Dave6.ShooterFramework
{
    /// <summary>
    /// 자체 로그 클래스
    /// </summary>
    public static class DaveLog
    {
        /// <summary>
        /// 이벤트나 상황이 발생
        /// </summary>
        public static void WhatHappend() { Internal_Log("뭔가 있는데", LogType.Log); }
        /// <summary>
        /// 뭐냐면..
        /// </summary>
        /// <param name="toPrint"></param>
        public static void WhatHappend(object toPrint)
        {
            if (toPrint == null) toPrint = "없어?";

            string message = "뭐냐면..: " + toPrint;
            Internal_Log(message, LogType.Log);
        }
        /// <summary>
        /// 사소한 찐빠가 있음
        /// </summary>
        public static void AttentionPlease() { Internal_Log("옐로카드", LogType.Warning); }
        /// <summary>
        /// 이거 경고임.
        /// </summary>
        /// <param name="warning"></param>
        public static void AttentionPlease(object warning)
        {
            // Null check.
            if (warning == null) warning = "아무튼";

            string message = "이거 경고임: " + warning;
            Internal_Log(message, LogType.Warning);
        }
        /// <summary>
        /// 문제가 있어요
        /// </summary>
        public static void ErrorAlert() { Internal_Log("오 쉣", LogType.Error); }
        /// <summary>
        /// 문제가 생겼는데.
        /// </summary>
        /// <param name="error"></param>
        public static void ErrorAlert(object error)
        {
            // Null check.
            if (error == null) error = "뭔지 모르겠음";

            string message = "문제가 생겼는데: " + error;
            Internal_Log(message, LogType.Error);
        }
        /// <summary>
        /// 뭔가 예외사항 발생
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="context"></param>
        public static void OopsException(Exception exception, UnityEngine.Object context = default(UnityEngine.Object))
        {
            Debug.LogException(exception, context);
        }

        private static void Internal_Log(string message, LogType type)
        {
            // Null case.
            if (message == " ")
                message = "Null";

            switch (type)
            {
                case LogType.Log:
                    Debug.Log(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                case LogType.Assert:
                    break;
                case LogType.Exception:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(type.GetType().FullName, type, null);
            }
        }
    }
}
