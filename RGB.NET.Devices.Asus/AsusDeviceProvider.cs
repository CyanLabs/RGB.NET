﻿// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.InteropServices;
using RGB.NET.Core;
using RGB.NET.Devices.Asus.Native;

namespace RGB.NET.Devices.Asus
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a device provider responsible for Cooler Master devices.
    /// </summary>
    public class AsusDeviceProvider : IRGBDeviceProvider
    {
        #region Properties & Fields

        private static AsusDeviceProvider _instance;
        /// <summary>
        /// Gets the singleton <see cref="AsusDeviceProvider"/> instance.
        /// </summary>
        public static AsusDeviceProvider Instance => _instance ?? new AsusDeviceProvider();

        /// <summary>
        /// Gets a modifiable list of paths used to find the native SDK-dlls for x86 applications.
        /// The first match will be used.
        /// </summary>
        public static List<string> PossibleX86NativePaths { get; } = new List<string> { "x86/AURA_SDK.dll" };

        /// <summary>
        /// Gets a modifiable list of paths used to find the native SDK-dlls for x64 applications.
        /// The first match will be used.
        /// </summary>
        public static List<string> PossibleX64NativePaths { get; } = new List<string> { };

        /// <inheritdoc />
        /// <summary>
        /// Indicates if the SDK is initialized and ready to use.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets the loaded architecture (x64/x86).
        /// </summary>
        public string LoadedArchitecture => _AsusSDK.LoadedArchitecture;

        /// <inheritdoc />
        /// <summary>
        /// Gets whether the application has exclusive access to the SDK or not.
        /// </summary>
        public bool HasExclusiveAccess { get; private set; }

        /// <inheritdoc />
        public IEnumerable<IRGBDevice> Devices { get; private set; }

        /// <summary>
        /// Gets or sets a function to get the culture for a specific device.
        /// </summary>
        public Func<CultureInfo> GetCulture { get; set; } = CultureHelper.GetCurrentCulture;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AsusDeviceProvider"/> class.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if this constructor is called even if there is already an instance of this class.</exception>
        public AsusDeviceProvider()
        {
            if (_instance != null) throw new InvalidOperationException($"There can be only one instanc of type {nameof(AsusDeviceProvider)}");
            _instance = this;
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public bool Initialize(bool exclusiveAccessIfPossible = false, bool throwExceptions = false)
        {
            IsInitialized = false;

            try
            {
                _AsusSDK.Reload();

                IList<IRGBDevice> devices = new List<IRGBDevice>();

                #region Mainboard

                //TODO DarthAffe 21.10.2017: Requesting mainboards seems to fail if only a non mb-device (tested with only a gpu) is connected
                int mainboardCount = _AsusSDK.EnumerateMbController(IntPtr.Zero, 0);
                if (mainboardCount > 0)
                {
                    IntPtr mainboardHandles = Marshal.AllocHGlobal(mainboardCount * IntPtr.Size);
                    _AsusSDK.EnumerateMbController(mainboardHandles, mainboardCount);

                    for (int i = 0; i < mainboardCount; i++)
                    {
                        IntPtr handle = Marshal.ReadIntPtr(mainboardHandles, i);
                        _AsusSDK.SetMbMode(handle, 1);
                        AsusMainboardRGBDevice device = new AsusMainboardRGBDevice(new AsusMainboardRGBDeviceInfo(RGBDeviceType.Mainboard, handle));
                        device.Initialize();
                        devices.Add(device);
                    }
                }

                #endregion

                #region Graphics cards

                //TODO DarthAffe 21.10.2017: This somehow returns non-existant gpus (at least for me) which cause huge lags (if a real asus-ready gpu is connected this doesn't happen)
                //int graphicCardCount = _AsusSDK.EnumerateGPU(IntPtr.Zero, 0);
                //if (graphicCardCount > 0)
                //{
                //    IntPtr grapicsCardHandles = Marshal.AllocHGlobal(graphicCardCount * IntPtr.Size);
                //    _AsusSDK.EnumerateGPU(grapicsCardHandles, graphicCardCount);

                //    for (int i = 0; i < graphicCardCount; i++)
                //    {
                //        IntPtr handle = Marshal.ReadIntPtr(grapicsCardHandles, i);
                //        _AsusSDK.SetGPUMode(handle, 1);
                //        AsusGraphicsCardRGBDevice device = new AsusGraphicsCardRGBDevice(new AsusGraphicsCardRGBDeviceInfo(RGBDeviceType.GraphicsCard, handle));
                //        device.Initialize();
                //        devices.Add(device);
                //    }
                //}

                #endregion

                //#region DRAM

                ////TODO DarthAffe 29.10.2017: I don't know why they are even documented, but the asus guy said they aren't in the SDK right now.
                ////int dramCount = _AsusSDK.EnumerateDram(IntPtr.Zero, 0);
                ////if (dramCount > 0)
                ////{
                ////    IntPtr dramHandles = Marshal.AllocHGlobal(dramCount * IntPtr.Size);
                ////    _AsusSDK.EnumerateDram(dramHandles, dramCount);

                ////    for (int i = 0; i < dramCount; i++)
                ////    {
                ////        IntPtr handle = Marshal.ReadIntPtr(dramHandles, i);
                ////        _AsusSDK.SetDramMode(handle, 1);
                ////        AsusDramRGBDevice device = new AsusDramRGBDevice(new AsusDramRGBDeviceInfo(RGBDeviceType.DRAM, handle));
                ////        device.Initialize();
                ////        devices.Add(device);
                ////    }
                ////}

                //#endregion

                //#region Keyboard

                //IntPtr keyboardHandle = Marshal.AllocHGlobal(IntPtr.Size);
                //if (_AsusSDK.CreateClaymoreKeyboard(keyboardHandle))
                //{
                //    _AsusSDK.SetClaymoreKeyboardMode(keyboardHandle, 1);
                //    AsusKeyboardRGBDevice device = new AsusKeyboardRGBDevice(new AsusKeyboardRGBDeviceInfo(RGBDeviceType.Keyboard, keyboardHandle, GetCulture()));
                //    device.Initialize();
                //    devices.Add(device);
                //}

                //#endregion

                //#region Mouse

                //IntPtr mouseHandle = Marshal.AllocHGlobal(IntPtr.Size);
                //if (_AsusSDK.CreateRogMouse(mouseHandle))
                //{
                //    _AsusSDK.SetRogMouseMode(mouseHandle, 1);
                //    AsusMouseRGBDevice device = new AsusMouseRGBDevice(new AsusMouseRGBDeviceInfo(RGBDeviceType.Mouse, mouseHandle));
                //    device.Initialize();
                //    devices.Add(device);
                //}

                //#endregion

                Devices = new ReadOnlyCollection<IRGBDevice>(devices);
            }
            catch
            {
                if (throwExceptions)
                    throw;
                else
                    return false;
            }

            IsInitialized = true;

            return true;
        }

        /// <inheritdoc />
        public void ResetDevices()
        {
            foreach (IRGBDevice device in Devices)
            {
                AsusRGBDeviceInfo deviceInfo = (AsusRGBDeviceInfo)device.DeviceInfo;
                switch (deviceInfo.DeviceType)
                {
                    case RGBDeviceType.Mainboard:
                        _AsusSDK.SetMbMode(deviceInfo.Handle, 0);
                        break;
                    case RGBDeviceType.GraphicsCard:
                        _AsusSDK.SetGPUMode(deviceInfo.Handle, 0);
                        break;
                    //case RGBDeviceType.DRAM:
                    //    _AsusSDK.SetDramMode(deviceInfo.Handle, 0);
                    //    break;
                }
            }
        }

        #endregion
    }
}
