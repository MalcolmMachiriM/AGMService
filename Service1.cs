using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AGMService
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer1 = (Timer)null;
        public Service1()
        {
            InitializeComponent();
        }


        protected override void OnStart(string[] args)
        {
            this.timer1 = new Timer();
            this.timer1.Interval = 1000.0; // 60000=5 minutes 120 minutes = 7200000.0  

            this.timer1.Elapsed += new ElapsedEventHandler(this.timer1_tick);
            this.timer1.Enabled = true;
            LogScriptor.WriteErrorLog("Agm App Service has started");
        }


        protected void SendUpdatetoAdmin()
        {
            try
            {
                //1. Insert new queries to admin
                //LogScriptor.WriteErrorLog("SendUpdatetoAdmin");
              
                AGMSyncing obj = new AGMSyncing("cn", 1);
                DataSet agmquery = obj.getQueryFromAgmPortal((int)LookUp.AGMRegisstration);//agm query
                if (agmquery != null)
                {
                    foreach (DataRow item in agmquery.Tables[0].Rows)
                    {

                        int RwID = Convert.ToInt32(item["ID"].ToString());
                        obj.ID = 0;
                        obj.PortalID = Convert.ToInt32(item["PortalID"].ToString());
                        obj.EventID = Convert.ToInt32(item["EventID"].ToString());
                        obj.RegistrationID = Convert.ToInt32(item["RegistrationID"].ToString());
                        //obj.DateCreated = item["DateCreated"].ToString();
                        obj.isSolved = Convert.ToBoolean(item["isSolved"].ToString());
                        obj.Query = item["Query"].ToString();
                        obj.QueryType = item["QueryType"].ToString();
                        obj.Comment = item["Comment"].ToString();
                        obj.ActionType = item["ActionType"].ToString();
                        obj.SyncID = 2;
                        if (obj.SavetoAdmin())
                        {
                            //2. Update sync status of what has been posted to admin in the portal system

                            LogScriptor.WriteErrorLog("Update Sync status on portal db to 2 for rec id: " + item["PortalID"].ToString());
                            if (obj.UpdateFromAdmin(RwID, 2)) //Update Portal SYNC status to 2 for admin update
                            {
                                LogScriptor.WriteErrorLog("Portal Rec Sync complete");
                            }
                            else
                            {
                                LogScriptor.WriteErrorLog("Portal Rec Sync Error:" + obj.Msgflg);
                            }
                        }
                        else
                        {
                            LogScriptor.WriteErrorLog("Error reported @ syncing a portal query to the live db: " + obj.Msgflg);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogScriptor.WriteErrorLog("Error reported @ sending update to admin system: " + ex.Message);
            }
        }


        protected void SyncUpdateToPortal()
        {
            try
            {
                //1. Updating Portal from admin 
                
                AGMSyncing obj = new AGMSyncing("cn", 1);
                DataSet agmquery = obj.getQueryFromAdmin((int)LookUp.AdminResponse);//admin query
                if (agmquery != null)
                {
                    foreach (DataRow item in agmquery.Tables[0].Rows)
                    {

                        obj.ID = Convert.ToInt32(item["PortalID"].ToString());
                        obj.EventID = Convert.ToInt32(item["EventID"].ToString());
                        obj.RegistrationID = Convert.ToInt32(item["RegistrationID"].ToString());
                        //obj.DateCreated = item["DateCreated"].ToString();
                        obj.isSolved = Convert.ToBoolean(item["isSolved"].ToString());
                        obj.Query = item["Query"].ToString();
                        obj.QueryType = item["QueryType"].ToString();
                        obj.Comment = item["Comment"].ToString();
                        obj.ActionType = item["ActionType"].ToString();
                        obj.SyncID = 4;
                        if (obj.SavePortalUpdate())
                        {
                            //2. Update sync status of what has been update in the admin db to the portal db
                            int RwID = Convert.ToInt32(item["PortalID"].ToString());
                            LogScriptor.WriteErrorLog("Update Sync status on portal db to 4 for rec id: " + item["PortalID"].ToString());
                            if (obj.UpdateLiveDB(RwID, 4))
                            {
                                LogScriptor.WriteErrorLog("Admin Rec Sync complete");
                            }
                            else
                            {
                                LogScriptor.WriteErrorLog("Admin Rec Sync Error:" + obj.Msgflg);
                            }
                        }
                        else
                        {
                            LogScriptor.WriteErrorLog("Error reported @ syncing a Admin query to the Portal db: " + obj.Msgflg);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogScriptor.WriteErrorLog("Error reported @ sending update to admin system: " + ex.Message);
            }
        }
        private void timer1_tick(object Sender, ElapsedEventArgs e)
        {
            try
            {
                
                SendUpdatetoAdmin();
               SyncUpdateToPortal();

            }
            catch (Exception ex)
            {
                LogScriptor.WriteErrorLog("Error reported @ getting new open batch header: " + ex.Message);
            }
        }

        protected override void OnStop()
        {
            this.timer1.Enabled = false;
            LogScriptor.WriteErrorLog("Agm  service stopped");
        }

        private void SrvcAction(object Sender, ElapsedEventArgs e)
        {

        }
    }
}
