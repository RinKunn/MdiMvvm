﻿using System;
using System.Collections.Generic;
using MdiMvvm.AppCore.Tests.Services.Mocks;
using MdiMvvm.AppCore.Services.WindowsServices.WindowsManager;

namespace MdiMvvm.AppCore.Tests.Services
{
    public class WindowsSamples
    {
        public List<KeyValuePair<Guid, List<Guid>>> Guids;

        public WindowsSamples(IWindowsManagerService manager)
        {
            Guids = new List<KeyValuePair<Guid, List<Guid>>>();
            for (int i = 1; i <= 3; i++)
            {
                MdiContainerMock container = new MdiContainerMock() { Title = "Title" + i };
                List<Guid> winds = new List<Guid>();
                for (int j = i; j <= 3; j++)
                {
                    var win = new MdiWindowMock() { Title = $"Window{i}{j}" , InternalText = "Internal text"};
                    winds.Add(win.Guid);
                    container.AddMdiWindow(win);
                }
                Guids.Add(new KeyValuePair<Guid, List<Guid>>(container.Guid, winds));
                manager.AppendContainer(container);
            }
            manager.ActivateContainer(Guids[0].Key);
        }
    }
}
