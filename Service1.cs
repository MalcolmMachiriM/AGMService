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

        private void timer1_tick(object Sender, ElapsedEventArgs e)
        {
            try
            {
                
                SendUpdatetoAdmin();
               SyncUpdateToPortal();
                GetMemberUploadsPortal();
                SyncMemberUpdateToPortal();
                SendContributionUpdatetoAdmin();
                SyncContributionUpdateToPortal();

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
                        obj.SyncID = (int)LookUp.AdminUpdated;
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
                LogScriptor.WriteErrorLog("Error reported @ SendUpdatetoAdmin() sending update to admin system: " + ex.Message);
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
                if (agmquery != null && agmquery.Tables.Count > 0 && agmquery.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow item in agmquery.Tables[0].Rows)
                    {

                        obj1.ID = Convert.ToInt32(item["PortalID"].ToString());
                        obj1.PensionNo = Convert.ToInt32(item["PensionNo"].ToString());
                        obj1.Description = item["Description"].ToString();
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
                        obj1.SyncID = (int)LookUp.AGMUpdate;
                        obj1.PortalID = Convert.ToInt32(item["PortalID"].ToString());


                        obj.ID = Convert.ToInt32(item["PortalID"].ToString());
                        obj.PensionNo = Convert.ToInt32(item["PensionNo"].ToString());
                        obj.Description = item["Description"].ToString();
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
                        obj.SyncID = (int)LookUp.AGMUpdate;
                        obj.PortalID = Convert.ToInt32(item["PortalID"].ToString());
                        if (obj1.SavePortalUpdate())
                        {
                            //2. Update sync status of what has been update in the admin db to the portal db
                            int RwID = Convert.ToInt32(item["PortalID"].ToString());
                            LogScriptor.WriteErrorLog("Update Sync status on portal db to 4 for rec id: " + item["PortalID"].ToString());
                            if (obj.UpdateLiveDB(RwID, (int)LookUp.AGMUpdate))
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
                LogScriptor.WriteErrorLog("Error reported @ SyncUpdateToPortal() sending update to admin system: " + ex.Message);
            }
        }
        #endregion

        #region member uploads


        protected void GetMemberUploadsPortal()
        {
            try
            {
                SyncingMemberUploads obj = new SyncingMemberUploads("cn", 1);
                SyncingMemberUploads obj1 = new SyncingMemberUploads("penAdmin", 1);
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
                        obj.msrepl_tran_version = Guid.Parse(item["msrepl_tran_version"].ToString());
                        obj.SplittedRegNo = item["SplittedRegNo"].ToString();
                        obj.OldNumber = Convert.ToDouble(item["OldNumber"].ToString());
                        obj.IdentityTypeID = Convert.ToInt32(item["IdentityTypeID"].ToString());
                        obj.ClientTypeID = Convert.ToInt32(item["ClientTypeID"].ToString());
                        obj.FundID = Convert.ToInt32(item["FundID"].ToString());
                        obj.JobTitleID = Convert.ToInt32(item["JobTitleID"].ToString());
                        obj.Isprocessed = Convert.ToBoolean(item["Isprocessed"].ToString());
                        obj.ProcessId = Convert.ToInt32(item["ProcessId"].ToString());
                        obj.PortalID = Convert.ToInt32(item["ID"].ToString());


                        int RwID1 = Convert.ToInt32(item["ID"].ToString());
                        obj1.PortalID = Convert.ToInt32(item["PortalID"].ToString());
                        obj1.PensionNo = item["PensionNo"].ToString();
                        obj1.EmployeeReferenceNumber = item["EmployeeReferenceNumber"].ToString();
                        obj1.CompanyNo = Convert.ToInt32(item["CompanyNo"].ToString());
                        obj1.BranchId = Convert.ToInt32(item["BranchId"].ToString());
                        obj1.CostCentre = item["CostCentre"].ToString();
                        obj1.DepartmentCode = item["DepartmentCode"].ToString();
                        obj1.LastName = item["LastName"].ToString();
                        obj1.FirstName = item["FirstName"].ToString();
                        obj1.DateOfBirth = Convert.ToDateTime(item["DateOfBirth"].ToString());
                        obj1.DOBConfirmed = Convert.ToBoolean(item["DOBConfirmed"].ToString());
                        obj1.Gender_ID = item["Gender_ID"].ToString();
                        obj1.IdentityNo = item["IdentityNo"].ToString();
                        obj1.FundCategory_ID = Convert.ToInt32(item["FundCategory_ID"].ToString());
                        obj1.MaritalStatus_ID = Convert.ToInt32(item["MaritalStatus_ID"].ToString());
                        obj1.DateJoinedCompany = Convert.ToDateTime(item["DateJoinedCompany"].ToString());
                        obj1.DateJoinedFund = Convert.ToDateTime(item["DateJoinedFund"].ToString());
                        obj1.PensionableServiceDate = Convert.ToDateTime(item["PensionableServiceDate"].ToString());
                        obj1.TranferInDate = Convert.ToDateTime(item["TranferInDate"].ToString());
                        obj1.NormalRetAge = Convert.ToInt32(item["NormalRetAge"].ToString());
                        obj1.AnnualSalary = Convert.ToDouble(item["AnnualSalary"].ToString());
                        obj1.PassportNo = item["PassportNo"].ToString();
                        obj1.TaxNo = item["TaxNo"].ToString();
                        obj1.Title_Id = Convert.ToInt32(item["Title_Id"].ToString());
                        obj1.MonthsWaiting = Convert.ToInt32(item["MonthsWaiting"].ToString());
                        obj1.DateSuspended = Convert.ToDateTime(item["DateSuspended"].ToString());
                        obj1.DateUnsuspended = Convert.ToDateTime(item["DateUnsuspended"].ToString());
                        obj1.DateOfExit = Convert.ToDateTime(item["DateOfExit"].ToString());
                        obj1.IntExitCode = Convert.ToInt32(item["IntExitCode"].ToString());
                        obj1.ExitCode = Convert.ToInt32(item["ExitCode"].ToString());
                        obj1.ChequeReqDateExitCode = Convert.ToDateTime(item["ChequeReqDateExitCode"].ToString());
                        obj1.EntryPostedDate = Convert.ToDateTime(item["EntryPostedDate"].ToString());
                        obj1.ExitLetterDate = Convert.ToDateTime(item["ExitLetterDate"].ToString());
                        obj1.Company_ID = Convert.ToInt32(item["Company_ID"].ToString());
                        obj1.Authorised = Convert.ToBoolean(item["Authorised"].ToString());
                        obj1.AuthorisedBy = Convert.ToInt32(item["AuthorisedBy"].ToString());
                        obj1.DateAuthorised = Convert.ToDateTime(item["DateAuthorised"].ToString());
                        obj1.DateModified = Convert.ToDateTime(item["DateModified"].ToString());
                        obj1.ModifiedBy = Convert.ToInt32(item["ModifiedBy"].ToString());
                        obj1.Active = Convert.ToBoolean(item["Active"].ToString());
                        obj1.UploadedBy = Convert.ToInt32(item["UploadedBy"].ToString());
                        obj1.DateUploaded = Convert.ToDateTime(item["DateUploaded"].ToString());
                        obj1.StartupMember = Convert.ToDouble(item["StartupMember"].ToString());
                        obj1.StartupEmployer = Convert.ToDouble(item["StartupEmployer"].ToString());
                        obj1.TotalStartup = Convert.ToDouble(item["TotalStartup"].ToString());
                        obj1.IsDeferred = Convert.ToBoolean(item["IsDeferred"].ToString());
                        obj1.Comments = item["Comments"].ToString();
                        obj1.InterBranchTransferDate = Convert.ToDateTime(item["InterBranchTransferDate"].ToString());
                        obj1.msrepl_tran_version = Guid.Parse(item["msrepl_tran_version"].ToString());
                        obj1.SplittedRegNo = item["SplittedRegNo"].ToString();
                        obj1.OldNumber = Convert.ToDouble(item["OldNumber"].ToString());
                        obj1.IdentityTypeID = Convert.ToInt32(item["IdentityTypeID"].ToString());
                        obj1.ClientTypeID = Convert.ToInt32(item["ClientTypeID"].ToString());
                        obj1.FundID = Convert.ToInt32(item["FundID"].ToString());
                        obj1.JobTitleID = Convert.ToInt32(item["JobTitleID"].ToString());
                        obj1.Isprocessed = Convert.ToBoolean(item["Isprocessed"].ToString());
                        obj1.ProcessId = Convert.ToInt32(item["ProcessId"].ToString());
                        obj1.PortalID = Convert.ToInt32(item["ID"].ToString());

                        if (obj1.SaveMemberUploadsToAdmin())
                        {
                            //2. Update sync status of what has been posted to admin in the portal system

                            LogScriptor.WriteErrorLog("@ GetMemberUploadsPortal() Update Sync status on portal db to 2 for rec id: " + item["PortalID"].ToString());
                            if (obj1.UpdateFromAdmin(RwID1, 2)) //Update Portal SYNC status to 2 for admin update
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
                            LogScriptor.WriteErrorLog("Error reported @ GetMemberUploadsPortal() syncing  portal members to the live db: " + obj.Msgflg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogScriptor.WriteErrorLog(ex.Message);
                LogScriptor.WriteErrorLog("Error reported @ GetMemberUploadsPortal() sending Member update to admin system: " + ex.Message);
            }
        }



        protected void SyncMemberUpdateToPortal()
        {
            try
            {
                //1. Updating Portal from admin 

                SyncingMemberUploads obj = new SyncingMemberUploads("cn", 1);
                SyncingMemberUploads obj1 = new SyncingMemberUploads("cn", 1);
                DataSet memberUploads = obj1.getMembersFromAdmin((int)LookUp.AdminResponse);//admin member
                if (memberUploads != null && memberUploads.Tables.Count > 0 && memberUploads.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow item in memberUploads.Tables[0].Rows)
                    {

                        int RwID1 = Convert.ToInt32(item["ID"].ToString());
                        obj1.PensionNo = item["PensionNo"].ToString();
                        obj1.EmployeeReferenceNumber = item["EmployeeReferenceNumber"].ToString();
                        obj1.CompanyNo = Convert.ToInt32(item["CompanyNo"].ToString());
                        obj1.BranchId = Convert.ToInt32(item["BranchId"].ToString());
                        obj1.CostCentre = item["CostCentre"].ToString();
                        obj1.DepartmentCode = item["DepartmentCode"].ToString();
                        obj1.LastName = item["LastName"].ToString();
                        obj1.FirstName = item["FirstName"].ToString();
                        obj1.DateOfBirth = Convert.ToDateTime(item["DateOfBirth"].ToString());
                        obj1.DOBConfirmed = Convert.ToBoolean(item["DOBConfirmed"].ToString());
                        obj1.Gender_ID = item["Gender_ID"].ToString();
                        obj1.IdentityNo = item["IdentityNo"].ToString();
                        obj1.FundCategory_ID = Convert.ToInt32(item["FundCategory_ID"].ToString());
                        obj1.MaritalStatus_ID = Convert.ToInt32(item["MaritalStatus_ID"].ToString());
                        obj1.DateJoinedCompany = Convert.ToDateTime(item["DateJoinedCompany"].ToString());
                        obj1.DateJoinedFund = Convert.ToDateTime(item["DateJoinedFund"].ToString());
                        obj1.PensionableServiceDate = Convert.ToDateTime(item["PensionableServiceDate"].ToString());
                        obj1.TranferInDate = Convert.ToDateTime(item["TranferInDate"].ToString());
                        obj1.NormalRetAge = Convert.ToInt32(item["NormalRetAge"].ToString());
                        obj1.AnnualSalary = Convert.ToDouble(item["AnnualSalary"].ToString());
                        obj1.PassportNo = item["PassportNo"].ToString();
                        obj1.TaxNo = item["TaxNo"].ToString();
                        obj1.Title_Id = Convert.ToInt32(item["Title_Id"].ToString());
                        obj1.MonthsWaiting = Convert.ToInt32(item["MonthsWaiting"].ToString());
                        obj1.DateSuspended = Convert.ToDateTime(item["DateSuspended"].ToString());
                        obj1.DateUnsuspended = Convert.ToDateTime(item["DateUnsuspended"].ToString());
                        obj1.DateOfExit = Convert.ToDateTime(item["DateOfExit"].ToString());
                        obj1.IntExitCode = Convert.ToInt32(item["IntExitCode"].ToString());
                        obj1.ExitCode = Convert.ToInt32(item["ExitCode"].ToString());
                        obj1.ChequeReqDateExitCode = Convert.ToDateTime(item["ChequeReqDateExitCode"].ToString());
                        obj1.EntryPostedDate = Convert.ToDateTime(item["EntryPostedDate"].ToString());
                        obj1.ExitLetterDate = Convert.ToDateTime(item["ExitLetterDate"].ToString());
                        obj1.Company_ID = Convert.ToInt32(item["Company_ID"].ToString());
                        obj1.Authorised = Convert.ToBoolean(item["Authorised"].ToString());
                        obj1.AuthorisedBy = Convert.ToInt32(item["AuthorisedBy"].ToString());
                        obj1.DateAuthorised = Convert.ToDateTime(item["DateAuthorised"].ToString());
                        obj1.DateModified = Convert.ToDateTime(item["DateModified"].ToString());
                        obj1.ModifiedBy = Convert.ToInt32(item["ModifiedBy"].ToString());
                        obj1.Active = Convert.ToBoolean(item["Active"].ToString());
                        obj1.UploadedBy = Convert.ToInt32(item["UploadedBy"].ToString());
                        obj1.DateUploaded = Convert.ToDateTime(item["DateUploaded"].ToString());
                        obj1.StartupMember = Convert.ToDouble(item["StartupMember"].ToString());
                        obj1.StartupEmployer = Convert.ToDouble(item["StartupEmployer"].ToString());
                        obj1.TotalStartup = Convert.ToDouble(item["TotalStartup"].ToString());
                        obj1.IsDeferred = Convert.ToBoolean(item["IsDeferred"].ToString());
                        obj1.Comments = item["Comments"].ToString();
                        obj1.InterBranchTransferDate = Convert.ToDateTime(item["InterBranchTransferDate"].ToString());
                        obj1.msrepl_tran_version = Guid.Parse(item["msrepl_tran_version"].ToString());
                        obj1.SplittedRegNo = item["SplittedRegNo"].ToString();
                        obj1.OldNumber = Convert.ToDouble(item["OldNumber"].ToString());
                        obj1.IdentityTypeID = Convert.ToInt32(item["IdentityTypeID"].ToString());
                        obj1.ClientTypeID = Convert.ToInt32(item["ClientTypeID"].ToString());
                        obj1.FundID = Convert.ToInt32(item["FundID"].ToString());
                        obj1.JobTitleID = Convert.ToInt32(item["JobTitleID"].ToString());
                        obj1.Isprocessed = Convert.ToBoolean(item["Isprocessed"].ToString());
                        obj1.ProcessId = Convert.ToInt32(item["ProcessId"].ToString());
                        obj1.PortalID = Convert.ToInt32(item["ID"].ToString());


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
                        obj.msrepl_tran_version = Guid.Parse(item["msrepl_tran_version"].ToString());
                        obj.SplittedRegNo = item["SplittedRegNo"].ToString();
                        obj.OldNumber = Convert.ToDouble(item["OldNumber"].ToString());
                        obj.IdentityTypeID = Convert.ToInt32(item["IdentityTypeID"].ToString());
                        obj.ClientTypeID = Convert.ToInt32(item["ClientTypeID"].ToString());
                        obj.FundID = Convert.ToInt32(item["FundID"].ToString());
                        obj.JobTitleID = Convert.ToInt32(item["JobTitleID"].ToString());
                        obj.Isprocessed = Convert.ToBoolean(item["Isprocessed"].ToString());
                        obj.ProcessId = Convert.ToInt32(item["ProcessId"].ToString());
                        obj.PortalID = Convert.ToInt32(item["ID"].ToString());
                        if (obj.SaveMemberUploadsToAdmin())
                        {
                            //2. Update sync status of what has been update in the admin db to the portal db
                            int RwID = Convert.ToInt32(item["PortalID"].ToString());
                            LogScriptor.WriteErrorLog("SyncMemberUpdateToPortal() Update Sync status on portal db to 4 for rec id: " + item["PortalID"].ToString());
                            if (obj.UpdateLiveDBMembers(RwID, (int)MemberUploads.PortalUpdate))
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
                            LogScriptor.WriteErrorLog("Error reported @ SyncMemberUpdateToPortal() syncing  Admin Members to the Portal db: " + obj.Msgflg);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogScriptor.WriteErrorLog("Error reported @ SyncMemberUpdateToPortal() sending update to admin system: " + ex.Message);
            }
        }

        #endregion


        #region Contributions
        protected void SendContributionUpdatetoAdmin()
        {
            try
            {
                //1. Insert new queries to admin
                //LogScriptor.WriteErrorLog("SendUpdatetoAdmin");

                ContributionSync obj = new ContributionSync("cn", 1);
                ContributionSync obj1 = new ContributionSync("penAdmin", 1);
                DataSet contributions = obj.getContributionsFromPortal((int)LookUp.AGMRegisstration);//agm query
                if (contributions != null && contributions.Tables.Count > 0 && contributions.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow item in contributions.Tables[0].Rows)
                    {

                        int RwID = int.Parse(item["ID"].ToString());
                        obj.ID = 0;
                        obj.MemberID = int.Parse(item["mMemberID"].ToString());
                        obj.NationalID = (item["NationalID"].ToString());
                        obj.Period = item["Period"].ToString();
                        obj.Salary = float.Parse(item["Salary"].ToString());
                        obj.RegNo = item["RegNo"].ToString();
                        obj.PaymentDate = Convert.ToDateTime(item["PaymentDate"].ToString());
                        obj.XContribution = float.Parse(item["XContribution"].ToString());
                        obj.YContribution = float.Parse(item["YContribution"].ToString());
                        obj.ZContribution = float.Parse(item["ZContribution"].ToString());
                        obj.GrossYContribution = float.Parse(item["GrossYContribution"].ToString());
                        obj.Expenses = float.Parse(item["Expenses"].ToString());
                        obj.ExpectedX = float.Parse(item["ExpectedX"].ToString());
                        obj.ExpectedY = float.Parse(item["ExpectedY"].ToString());
                        obj.UserID = int.Parse(item["UserID"].ToString());
                        obj.DateCaptured = DateTime.Parse(item["DateCaptured"].ToString());
                        obj.TransID = Guid.Parse(item["TransID"].ToString());
                        obj.BatchID = Guid.Parse(item["BatchID"].ToString());
                        obj.TransferInMember = float.Parse(item["TransferInMember"].ToString());
                        obj.TransferInEmployer = float.Parse(item["TransferInEmployer"].ToString());
                        obj.OtherMember = float.Parse(item["OtherMember"].ToString());
                        obj.OtherEmployer = float.Parse(item["OtherEmployer"].ToString());
                        obj.Total = float.Parse(item["Total"].ToString());
                        obj.isStartup = bool.Parse(item["isStartup"].ToString());
                        obj.BackPay = double.Parse(item["BackPay"].ToString());
                        obj.BranchCode = (item["BranchCode"].ToString());
                        obj.LatestUpdateDate = DateTime.Parse(item["LatestUpdateDate"].ToString());
                        obj.IsHistory = bool.Parse(item["IsHistory"].ToString());
                        obj.PeriodDate = DateTime.Parse(item["PeriodDate"].ToString());
                        obj.MyKey = (item["MyKey"].ToString());
                        obj.CompanyID = int.Parse(item["CompanyID"].ToString());
                        obj.SplittedRegNo = (item["SplittedRegNo"].ToString());
                        obj.SplittedMemberID = int.Parse(item["SplittedMemberID"].ToString());
                        obj.SplittedBrachCode = int.Parse(item["SplittedBrachCode"].ToString());
                        obj.DateSplitted = DateTime.Parse(item["DateSplitted"].ToString());
                        obj.BonusTypeID = int.Parse(item["BonusTypeID"].ToString());
                        obj.BatchNo = (item["BatchNo"].ToString());
                        obj.Platform = (item["Platform"].ToString());
                        obj.SyncID = 2;
                        obj.PortalID = Convert.ToInt32(item["PortalID"].ToString()); 
                        
                        
                        obj1.ID = 0;
                        obj1.MemberID = int.Parse(item["mMemberID"].ToString());
                        obj1.NationalID = (item["NationalID"].ToString());
                        obj1.Period = item["Period"].ToString();
                        obj1.Salary = float.Parse(item["Salary"].ToString());
                        obj1.RegNo = item["RegNo"].ToString();
                        obj1.PaymentDate = Convert.ToDateTime(item["PaymentDate"].ToString());
                        obj1.XContribution = float.Parse(item["XContribution"].ToString());
                        obj1.YContribution = float.Parse(item["YContribution"].ToString());
                        obj1.ZContribution = float.Parse(item["ZContribution"].ToString());
                        obj1.GrossYContribution = float.Parse(item["GrossYContribution"].ToString());
                        obj1.Expenses = float.Parse(item["Expenses"].ToString());
                        obj1.ExpectedX = float.Parse(item["ExpectedX"].ToString());
                        obj1.ExpectedY = float.Parse(item["ExpectedY"].ToString());
                        obj1.UserID = int.Parse(item["UserID"].ToString());
                        obj1.DateCaptured = DateTime.Parse(item["DateCaptured"].ToString());
                        obj1.TransID = Guid.Parse(item["TransID"].ToString());
                        obj1.BatchID = Guid.Parse(item["BatchID"].ToString());
                        obj1.TransferInMember = float.Parse(item["TransferInMember"].ToString());
                        obj1.TransferInEmployer = float.Parse(item["TransferInEmployer"].ToString());
                        obj1.OtherMember = float.Parse(item["OtherMember"].ToString());
                        obj1.OtherEmployer = float.Parse(item["OtherEmployer"].ToString());
                        obj1.Total = float.Parse(item["Total"].ToString());
                        obj1.isStartup = bool.Parse(item["isStartup"].ToString());
                        obj1.BackPay = double.Parse(item["BackPay"].ToString());
                        obj1.BranchCode = (item["BranchCode"].ToString());
                        obj1.LatestUpdateDate = DateTime.Parse(item["LatestUpdateDate"].ToString());
                        obj1.IsHistory = bool.Parse(item["IsHistory"].ToString());
                        obj1.PeriodDate = DateTime.Parse(item["PeriodDate"].ToString());
                        obj1.MyKey = (item["MyKey"].ToString());
                        obj1.CompanyID = int.Parse(item["CompanyID"].ToString());
                        obj1.SplittedRegNo = (item["SplittedRegNo"].ToString());
                        obj1.SplittedMemberID = int.Parse(item["SplittedMemberID"].ToString());
                        obj1.SplittedBrachCode = int.Parse(item["SplittedBrachCode"].ToString());
                        obj1.DateSplitted = DateTime.Parse(item["DateSplitted"].ToString());
                        obj1.BonusTypeID = int.Parse(item["BonusTypeID"].ToString());
                        obj1.BatchNo = (item["BatchNo"].ToString());
                        obj1.Platform = (item["Platform"].ToString());
                        obj1.SyncID = 2;
                        obj1.PortalID = Convert.ToInt32(item["PortalID"].ToString());

                        


                        if (obj1.SavetoAdmin())
                        {
                            //2. Update sync status of what has been posted to admin in the portal system

                            LogScriptor.WriteErrorLog("SendContributionUpdatetoAdmin() Update Sync status on portal db to 2 for rec id: " + item["PortalID"].ToString());
                            if (obj.UpdateFromAdmin(obj1.PortalID, ((int)ContributionsUploads.AdminUpdated))) //Update Portal SYNC status to 2 for admin update
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
                            LogScriptor.WriteErrorLog("Error reported @ SendContributionUpdatetoAdmin() syncing a Contributions to the live db: " + obj.Msgflg);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogScriptor.WriteErrorLog("Error reported @ SendContributionUpdatetoAdmin() sending update to admin system: " + ex.Message);
            }
        }

        protected void SyncContributionUpdateToPortal()
        {
            try
            {
                //1. Updating Portal from admin 

                ContributionSync obj = new ContributionSync("cn", 1);
                ContributionSync obj1 = new ContributionSync("cn", 1);
                DataSet contributions = obj1.getQueryFromAdmin((int)LookUp.AdminResponse);//admin query
                if (contributions != null && contributions.Tables.Count > 0 && contributions.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow item in contributions.Tables[0].Rows)
                    {

                        obj1.ID = Convert.ToInt32(item["PortalID"].ToString());
                        obj1.MemberID = int.Parse(item["mMemberID"].ToString());
                        obj1.NationalID = (item["NationalID"].ToString());
                        obj1.Period = item["Period"].ToString();
                        obj1.Salary = float.Parse(item["Salary"].ToString());
                        obj1.RegNo = item["RegNo"].ToString();
                        obj1.PaymentDate = Convert.ToDateTime(item["PaymentDate"].ToString());
                        obj1.XContribution = float.Parse(item["XContribution"].ToString());
                        obj1.YContribution = float.Parse(item["YContribution"].ToString());
                        obj1.ZContribution = float.Parse(item["ZContribution"].ToString());
                        obj1.GrossYContribution = float.Parse(item["GrossYContribution"].ToString());
                        obj1.Expenses = float.Parse(item["Expenses"].ToString());
                        obj1.ExpectedX = float.Parse(item["ExpectedX"].ToString());
                        obj1.ExpectedY = float.Parse(item["ExpectedY"].ToString());
                        obj1.UserID = int.Parse(item["UserID"].ToString());
                        obj1.DateCaptured = DateTime.Parse(item["DateCaptured"].ToString());
                        obj1.TransID = Guid.Parse(item["TransID"].ToString());
                        obj1.BatchID = Guid.Parse(item["BatchID"].ToString());
                        obj1.TransferInMember = float.Parse(item["TransferInMember"].ToString());
                        obj1.TransferInEmployer = float.Parse(item["TransferInEmployer"].ToString());
                        obj1.OtherMember = float.Parse(item["OtherMember"].ToString());
                        obj1.OtherEmployer = float.Parse(item["OtherEmployer"].ToString());
                        obj1.Total = float.Parse(item["Total"].ToString());
                        obj1.isStartup = bool.Parse(item["isStartup"].ToString());
                        obj1.BackPay = double.Parse(item["BackPay"].ToString());
                        obj1.BranchCode = (item["BranchCode"].ToString());
                        obj1.LatestUpdateDate = DateTime.Parse(item["LatestUpdateDate"].ToString());
                        obj1.IsHistory = bool.Parse(item["IsHistory"].ToString());
                        obj1.PeriodDate = DateTime.Parse(item["PeriodDate"].ToString());
                        obj1.MyKey = (item["MyKey"].ToString());
                        obj1.CompanyID = int.Parse(item["CompanyID"].ToString());
                        obj1.SplittedRegNo = (item["SplittedRegNo"].ToString());
                        obj1.SplittedMemberID = int.Parse(item["SplittedMemberID"].ToString());
                        obj1.SplittedBrachCode = int.Parse(item["SplittedBrachCode"].ToString());
                        obj1.DateSplitted = DateTime.Parse(item["DateSplitted"].ToString());
                        obj1.BonusTypeID = int.Parse(item["BonusTypeID"].ToString());
                        obj1.BatchNo = (item["BatchNo"].ToString());
                        obj1.Platform = (item["Platform"].ToString());
                        obj1.SyncID = 4;
                        obj1.PortalID = Convert.ToInt32(item["PortalID"].ToString());


                       obj.ID = Convert.ToInt32(item["PortalID"].ToString());
                       obj.MemberID = int.Parse(item["mMemberID"].ToString());
                       obj.NationalID = (item["NationalID"].ToString());
                       obj.Period = item["Period"].ToString();
                       obj.Salary = float.Parse(item["Salary"].ToString());
                       obj.RegNo = item["RegNo"].ToString();
                       obj.PaymentDate = Convert.ToDateTime(item["PaymentDate"].ToString());
                       obj.XContribution = float.Parse(item["XContribution"].ToString());
                       obj.YContribution = float.Parse(item["YContribution"].ToString());
                       obj.ZContribution = float.Parse(item["ZContribution"].ToString());
                       obj.GrossYContribution = float.Parse(item["GrossYContribution"].ToString());
                       obj.Expenses = float.Parse(item["Expenses"].ToString());
                       obj.ExpectedX = float.Parse(item["ExpectedX"].ToString());
                       obj.ExpectedY = float.Parse(item["ExpectedY"].ToString());
                       obj.UserID = int.Parse(item["UserID"].ToString());
                       obj.DateCaptured = DateTime.Parse(item["DateCaptured"].ToString());
                       obj.TransID = Guid.Parse(item["TransID"].ToString());
                       obj.BatchID = Guid.Parse(item["BatchID"].ToString());
                       obj.TransferInMember = float.Parse(item["TransferInMember"].ToString());
                       obj.TransferInEmployer = float.Parse(item["TransferInEmployer"].ToString());
                       obj.OtherMember = float.Parse(item["OtherMember"].ToString());
                       obj.OtherEmployer = float.Parse(item["OtherEmployer"].ToString());
                       obj.Total = float.Parse(item["Total"].ToString());
                       obj.isStartup = bool.Parse(item["isStartup"].ToString());
                       obj.BackPay = double.Parse(item["BackPay"].ToString());
                       obj.BranchCode = (item["BranchCode"].ToString());
                       obj.LatestUpdateDate = DateTime.Parse(item["LatestUpdateDate"].ToString());
                       obj.IsHistory = bool.Parse(item["IsHistory"].ToString());
                       obj.PeriodDate = DateTime.Parse(item["PeriodDate"].ToString());
                       obj.MyKey = (item["MyKey"].ToString());
                       obj.CompanyID = int.Parse(item["CompanyID"].ToString());
                       obj.SplittedRegNo = (item["SplittedRegNo"].ToString());
                       obj.SplittedMemberID = int.Parse(item["SplittedMemberID"].ToString());
                       obj.SplittedBrachCode = int.Parse(item["SplittedBrachCode"].ToString());
                       obj.DateSplitted = DateTime.Parse(item["DateSplitted"].ToString());
                       obj.BonusTypeID = int.Parse(item["BonusTypeID"].ToString());
                       obj.BatchNo = (item["BatchNo"].ToString());
                       obj.Platform = (item["Platform"].ToString());
                       obj.SyncID = 4;
                       obj.PortalID = Convert.ToInt32(item["PortalID"].ToString());
                        if (obj.SavePortalUpdate())
                        {
                            //2. Update sync status of what has been update in the admin db to the portal db
                            int RwID = Convert.ToInt32(item["PortalID"].ToString());
                            LogScriptor.WriteErrorLog("SyncContributionUpdateToPortal() Update Sync status on portal db to 4 for rec id: " + item["PortalID"].ToString());
                            if (obj.UpdateLiveDB(RwID, (int)(ContributionsUploads.PortalUpdate)))
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
                            LogScriptor.WriteErrorLog("Error reported @ SyncContributionUpdateToPortal() syncing a Admin Contributions to the Portal db: " + obj.Msgflg);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogScriptor.WriteErrorLog("Error reported @ SyncContributionUpdateToPortal() sending update to admin system: " + ex.Message);
            }
        }
        #endregion
    }
}
