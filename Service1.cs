using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Design.CommonDesignTime;

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

        #region Queries
        protected void SendUpdatetoAdmin()
        {
            try
            {
                //1. Insert new queries to admin
                //LogScriptor.WriteErrorLog("SendUpdatetoAdmin");
              
                AGMSyncing obj = new AGMSyncing("cn", 1);
                AGMSyncing obj1 = new AGMSyncing("penAdmin", 1);
                DataSet agmquery = obj.getQueryFromAgmPortal((int)LookUp.AGMRegisstration);//agm query
                if (agmquery != null && agmquery.Tables.Count > 0 && agmquery.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow item in agmquery.Tables[0].Rows)
                    {

                        int RwID = Convert.ToInt32(item["ID"].ToString());
                        obj.ID = 0;
                        obj.PensionNo = Convert.ToInt32(item["PensionNo"].ToString());
                        obj.Description = (item["Description"].ToString());
                        obj.City = item["City"].ToString();
                        obj.QueryType = item["QueryType"].ToString();
                        obj.ContentType = item["ContentType"].ToString();
                        obj.Data = Convert.ToByte(item["Data"].ToString());
                        obj.isSolved = Convert.ToBoolean(item["isSolved"].ToString());
                        obj.Subject = item["Subject"].ToString();
                        obj.RegNo = item["RegNo"].ToString();
                        obj.Type = item["Type"].ToString();
                        obj.Comment = item["Comment"].ToString();
                        obj.ActionType = item["ActionType"].ToString();
                        obj.DocumentName = item["DocumentName"].ToString();
                        obj.SyncID = 2;
                        obj.PortalID = Convert.ToInt32(item["PortalID"].ToString());

                        int RwID1 = Convert.ToInt32(item["ID"].ToString());
                        obj1.ID = 0;
                        obj1.PensionNo = Convert.ToInt32(item["PensionNo"].ToString());
                        obj1.Description = (item["Description"].ToString());
                        obj1.City = item["City"].ToString();
                        obj1.QueryType = item["QueryType"].ToString();
                        obj1.ContentType = item["ContentType"].ToString();
                        obj1.Data = Convert.ToByte(item["Data"].ToString());
                        obj1.isSolved = Convert.ToBoolean(item["isSolved"].ToString());
                        obj1.Subject = item["Subject"].ToString();
                        obj1.RegNo = item["RegNo"].ToString();
                        obj1.Type = item["Type"].ToString();
                        obj1.Comment = item["Comment"].ToString();
                        obj1.ActionType = item["ActionType"].ToString();
                        obj1.DocumentName = item["DocumentName"].ToString();
                        obj1.SyncID = 2;
                        obj1.PortalID = Convert.ToInt32(item["PortalID"].ToString());


                        if (obj1.SavetoAdmin())
                        {
                            //2. Update sync status of what has been posted to admin in the portal system
                            
                            LogScriptor.WriteErrorLog("Update Sync status on portal db to 2 for rec id: " + item["PortalID"].ToString());
                            if (obj.UpdateFromAdmin(obj1.PortalID, 2)) //Update Portal SYNC status to 2 for admin update
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
                AGMSyncing obj1 = new AGMSyncing("cn", 1);
                DataSet agmquery = obj1.getQueryFromAdmin((int)LookUp.AdminResponse);//admin query
                if (agmquery != null && agmquery.Tables.Count>0 && agmquery.Tables[0].Rows.Count>0)
                {
                    foreach (DataRow item in agmquery.Tables[0].Rows)
                    {

                        obj1.ID = Convert.ToInt32(item["PortalID"].ToString());
                        obj1.PortalID = Convert.ToInt32(item["PortalID"].ToString());
                        obj1.PensionNo = Convert.ToInt32(item["PensionNo"].ToString());
                        obj1.Description = (item["Description"].ToString());
                        obj1.City = item["City"].ToString();
                        obj1.QueryType = item["QueryType"].ToString();
                        obj1.ContentType = item["ContentType"].ToString();
                        obj1.Data = Convert.ToByte(item["Data"].ToString());
                        obj1.isSolved = Convert.ToBoolean(item["isSolved"].ToString());
                        obj1.Subject = item["Subject"].ToString();
                        obj1.RegNo = item["RegNo"].ToString();
                        obj1.Type = item["Type"].ToString();
                        obj1.Comment = item["Comment"].ToString();
                        obj1.ActionType = item["ActionType"].ToString();
                        obj1.DocumentName = item["DocumentName"].ToString();
                        obj1.SyncID = 4;
                        if (obj1.SavePortalUpdate())
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
        #endregion

        #region member uploads

      
        protected void GetMemberUploadsPortal()
        {
            try
            {
                SyncingMemberUploads obj = new SyncingMemberUploads("cn", 1);
                DataSet ups = obj.GetMemberUploads((int)MemberUploads.AddedToPortal);
                if (ups != null)
                {
                    foreach (DataRow item in ups.Tables[0].Rows)
                    {
                        int RwID = Convert.ToInt32(item["ID"].ToString());
                        obj.PortalID = Convert.ToInt32(item["PortalID"].ToString());
                        obj.PensionNo = item["PensionNo"].ToString();
                        obj.EmployeeReferenceNumber = item["EmployeeReferenceNumber"].ToString();
                        obj.CompanyNo = Convert.ToInt32(item["CompanyNo"].ToString());
                        obj.BranchId = Convert.ToInt32(item["BranchId"].ToString());
                        obj.CostCentre = item["CostCentre"].ToString();
                        obj.DepartmentCode = item["DepartmentCode"].ToString();
                        obj.LastName = item["LastName"].ToString();
                        obj.FirstName = item["FirstName"].ToString();
                        obj.DateOfBirth = Convert.ToDateTime(item["DateOfBirth"].ToString());
                        obj.DOBConfirmed = Convert.ToBoolean(item["DOBConfirmed"].ToString());
                        obj.Gender_ID = item["Gender_ID"].ToString();
                        obj.IdentityNo = item["IdentityNo"].ToString();
                        obj.FundCategory_ID = Convert.ToInt32(item["FundCategory_ID"].ToString());
                        obj.MaritalStatus_ID = Convert.ToInt32(item["MaritalStatus_ID"].ToString());
                        obj.DateJoinedCompany = Convert.ToDateTime(item["DateJoinedCompany"].ToString());
                        obj.DateJoinedFund = Convert.ToDateTime(item["DateJoinedFund"].ToString());
                        obj.PensionableServiceDate = Convert.ToDateTime(item["PensionableServiceDate"].ToString());
                        obj.TranferInDate = Convert.ToDateTime(item["TranferInDate"].ToString());
                        obj.NormalRetAge = Convert.ToInt32(item["NormalRetAge"].ToString());
                        obj.AnnualSalary = Convert.ToDouble(item["AnnualSalary"].ToString());
                        obj.PassportNo = item["PassportNo"].ToString();
                        obj.TaxNo = item["TaxNo"].ToString();
                        obj.Title_Id = Convert.ToInt32(item["Title_Id"].ToString());
                        obj.MonthsWaiting = Convert.ToInt32(item["MonthsWaiting"].ToString());
                        obj.DateSuspended = Convert.ToDateTime(item["DateSuspended"].ToString());
                        obj.DateUnsuspended = Convert.ToDateTime(item["DateUnsuspended"].ToString());
                        obj.DateOfExit = Convert.ToDateTime(item["DateOfExit"].ToString());
                        obj.IntExitCode = Convert.ToInt32(item["IntExitCode"].ToString());
                        obj.ExitCode = Convert.ToInt32(item["ExitCode"].ToString());
                        obj.ChequeReqDateExitCode = Convert.ToDateTime(item["ChequeReqDateExitCode"].ToString());
                        obj.EntryPostedDate = Convert.ToDateTime(item["EntryPostedDate"].ToString());
                        obj.ExitLetterDate = Convert.ToDateTime(item["ExitLetterDate"].ToString());
                        obj.Company_ID = Convert.ToInt32(item["Company_ID"].ToString());
                        obj.Authorised = Convert.ToBoolean(item["Authorised"].ToString());
                        obj.AuthorisedBy = Convert.ToInt32(item["AuthorisedBy"].ToString());
                        obj.DateAuthorised = Convert.ToDateTime(item["DateAuthorised"].ToString());
                        obj.DateModified = Convert.ToDateTime(item["DateModified"].ToString());
                        obj.ModifiedBy = Convert.ToInt32(item["ModifiedBy"].ToString());
                        obj.Active = Convert.ToBoolean(item["Active"].ToString());
                        obj.UploadedBy = Convert.ToInt32(item["UploadedBy"].ToString());
                        obj.DateUploaded = Convert.ToDateTime(item["DateUploaded"].ToString());
                        obj.StartupMember = Convert.ToDouble(item["StartupMember"].ToString());
                        obj.StartupEmployer = Convert.ToDouble(item["StartupEmployer"].ToString());
                        obj.TotalStartup = Convert.ToDouble(item["TotalStartup"].ToString());
                        obj.IsDeferred = Convert.ToBoolean(item["IsDeferred"].ToString());
                        obj.Comments = item["Comments"].ToString();
                        obj.InterBranchTransferDate = Convert.ToDateTime(item["InterBranchTransferDate"].ToString());
                        obj.msrepl_tran_version =Guid.Parse(item["msrepl_tran_version"].ToString());
                        obj.SplittedRegNo = item["SplittedRegNo"].ToString();
                        obj.OldNumber = Convert.ToDouble(item["OldNumber"].ToString());
                        obj.IdentityTypeID = Convert.ToInt32(item["IdentityTypeID"].ToString());
                        obj.ClientTypeID = Convert.ToInt32(item["ClientTypeID"].ToString());
                        obj.FundID = Convert.ToInt32(item["FundID"].ToString());
                        obj.JobTitleID = Convert.ToInt32(item["JobTitleID"].ToString());
                        obj.Isprocessed = Convert.ToBoolean(item["Isprocessed"].ToString());
                        obj.ProcessId = Convert.ToInt32(item["ProcessId"].ToString());

                        if (obj.SaveMemberUploadsToAdmin())
                        {
                            //2. Update sync status of what has been posted to admin in the portal system

                            LogScriptor.WriteErrorLog("Update Sync status on portal db to 2 for rec id: " + item["PortalID"].ToString());
                            if (obj.UpdateFromAdmin(RwID, 2)) //Update Portal SYNC status to 2 for admin update
                            {
                                LogScriptor.WriteErrorLog("Portal Members Sync complete");
                            }
                            else
                            {
                                LogScriptor.WriteErrorLog("Portal Members Sync Error:" + obj.Msgflg);
                            }
                        }
                        else
                        {
                            LogScriptor.WriteErrorLog("Error reported @ syncing  portal members to the live db: " + obj.Msgflg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogScriptor.WriteErrorLog(ex.Message);
                LogScriptor.WriteErrorLog("Error reported @ sending Member update to admin system: " + ex.Message);
            }
        }

        #endregion
        private void timer1_tick(object Sender, ElapsedEventArgs e)
        {
            try
            {
                
                SendUpdatetoAdmin();
               SyncUpdateToPortal();
                //GetMemberUploadsPortal();

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
