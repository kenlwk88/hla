using HLA.Backend.Core.Infra.FCM.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HLA.Backend.Core.Infra.FCM
{
    public interface IFcmFactory 
    {
        IFcmServices CreateService(string appSource);
    }
    public class FcmFactory : IFcmFactory
    {
        private readonly FcmHla360Services _fcmHla360Services;
        private readonly FcmEcrmServices _fcmEcrmServices;
        private readonly FcmEcrmBancaServices _fcmEcrmBancaServices;
        public FcmFactory(FcmHla360Services fcmHla360Services, FcmEcrmServices fcmEcrmServices, FcmEcrmBancaServices fcmEcrmBancaServices)
        {
            _fcmHla360Services = fcmHla360Services;
            _fcmEcrmServices = fcmEcrmServices;
            _fcmEcrmBancaServices = fcmEcrmBancaServices;
        }
        public IFcmServices CreateService(string appSource) 
        {
            switch (appSource)
            {
                case "HLA360":
                    return _fcmHla360Services;
                case "ECRM":
                    return _fcmEcrmServices;
                case "ECRM_BANCA":
                    return _fcmEcrmBancaServices;
                default:
                    throw new ArgumentException("Failed to create FCM Serivce");
            }
        }
    }
}
