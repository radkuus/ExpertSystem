using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.Services
{
    public interface IServerService
    {
        Task StartServer();
        Task StopServer();
        bool IsRunning();

    }
}
