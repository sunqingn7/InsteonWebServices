using System;
using System.Collections.Generic;
using Insteon.Daemon.Common.Settings;
using Insteon.Network.Device;
using Insteon.Network.Enum;
using Insteon.Network.Helpers;
using Insteon.ServiceModel.Request;
using Insteon.ServiceModel.Response;
using ServiceStack;
using SettingsProviderNet;

namespace Insteon.Daemon.Common.Service
{

    public class InsteonService : IService
    {
        private readonly InsteonManager manager;
        private readonly SmartThingsSettings settings;

        public InsteonService(InsteonManager manager, SmartThingsSettings settings)
        {
            this.manager = manager;
            this.settings = settings;
        }

        public object Get(ConnectToInsteonNetworkRequest request)
        {
            if (manager.Network.IsConnected)
                return new ResponseStatus("100", "Already connected");

            try
            {
                var connected = manager.Connect();
                if (!connected)
                {
                    return new ResponseStatus("101", "Cannot connect to Insteon Controller.");
                }
            }
            catch (Exception exception)
            {
                return new ResponseStatus("102", exception.Message);
            }

            return true;
        }

        public object Get(RefreshDeviceLinksRequest request)
        {
            if (!manager.Network.IsConnected)
                throw new Exception("Not Connected");
            
            manager.RefreshDeviceDatabase();

            return true;
        }

        public GetDevicesResponse Get(GetDevices request)
        {
            var result = new GetDevicesResponse() { Devices = new List<DeviceInfo>() };

            foreach (InsteonDevice device in manager.Network.Devices)
            {
                result.Devices.Add(new DeviceInfo()
                {
                    Address = device.Address.ToString(),
                    Category = device.Identity.GetDeviceCategoryName(),
                    SubCategory = device.Identity.GetSubCategoryName(),
                    Name = device.DeviceName
                });
            }

            return result;
        }

        public object Get(GetStatus request)
        {
            return new
            {
                Address = manager.Connection.Address.ToString(),
                manager.Connection.Name,
                manager.Connection.Type,
                manager.Network.IsConnected,
                manager.Network.Devices.Count
            };

        }

        public ResponseStatus Put(SmartThingsSettingsRequest request)
        {
            settings.AccessToken = request.AccessToken;
            settings.Location = request.Location;
            settings.ApplicationId = request.AppId;

            var settingsProvider = new SettingsProvider(new RoamingAppDataStorage("Insteon"));
            settingsProvider.SaveSettings(settings);

            var cb = new SmartThingsCallbacks(settings);

            return !cb.Authorization() ? new InsteonServiceResponse("404", "Couldn't connect to ST hub") : new InsteonServiceResponse();
        }

        public ResponseStatus Get(SmartThingsSettingsResetRequest request)
        {
            settings.AccessToken = null;
            settings.Location = null;
            settings.ApplicationId = null;

            var settingsProvider = new SettingsProvider(new RoamingAppDataStorage("Insteon"));
            settingsProvider.SaveSettings(settings);

            var cb = new SmartThingsCallbacks(settings);

            return !cb.AuthorizationRevoke() ? new ResponseStatus("404", "Couldn't connect to ST hub") : new ResponseStatus();
        }

        public ResponseStatus Put(EnterLinkModeRequest request)
        {
            if (request.Start && manager.Network.Controller.IsInLinkingMode)
            {
                return new ResponseStatus();
            }
            if (request.Start && !manager.Network.Controller.IsInLinkingMode)
            {
                manager.Network.Controller.EnterLinkMode(InsteonLinkMode.Controller, 0x01);
                return new ResponseStatus();
            }
            if (!request.Start && manager.Network.Controller.IsInLinkingMode)
            {
                manager.Network.Controller.CancelLinkMode();
                return new ResponseStatus();
            }

            return new ResponseStatus("404", "Cannot change to provided linking mode.");
        }


    }
}