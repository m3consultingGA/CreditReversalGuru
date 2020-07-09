using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CreditReversal.Models;
using CreditReversal.DAL;


namespace CreditReversal.BLL
{
    public class IdentityIQFunction
    {
        private IdentityIQData IQData = new IdentityIQData();
        long status = 0;

        public long InsertIdetityIQInfo(IdentityIQInfo IQInfo)
        {
            try
            {
                status = IQData.InsertIdentityIQInfo(IQInfo);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return status;
        }

        public long UpdateIdetityIQInfo(IdentityIQInfo IQInfo)
        {
            try
            {
                status = IQData.UpdateIdentityIQInfo(IQInfo);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return status;
        }


        public IdentityIQInfo GetIdentityIQInfo(string ClientId)        {            IdentityIQInfo identityIQInfo = new IdentityIQInfo();            try            {                identityIQInfo = IQData.GetIdentityIQInfo(ClientId);            }            catch (Exception ex)            {                System.Diagnostics.Debug.WriteLine(ex.Message);            }            return identityIQInfo;        }
        public bool CheckIdentityIQInfo(long ClientId)        {            bool status = false;            try            {                status = IQData.GetIdentityIQInfo(ClientId);            }            catch (Exception ex)            {                System.Diagnostics.Debug.WriteLine(ex.Message);            }            return status;        }
        public IdentityIQInfo CheckIdentityIQInfo(string ClientId)        {            IdentityIQInfo objIdentity = new IdentityIQInfo();            try            {                objIdentity = IQData.GetIdentityIQInfo(ClientId);            }            catch (Exception ex)            {                System.Diagnostics.Debug.WriteLine(ex.Message);            }            return objIdentity;        }
    }
}