using Microsoft.Practices.EnterpriseLibrary.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGMService
{

    public enum MemberUploads
    {
        AddedToPortal = 0,
        ApprovedInAdmin = 1,
        PortalUpdated = 3
    }
    class SyncingMemberUploads
    {
        #region variables
        private MySqlConnection cnxn;
        protected Database portalDB;
        protected Database liveDB;
        protected string mConString;
        protected string mLiveConString;
        protected string mPensionNo;
        protected string mEmployeeReferenceNumber;
        protected int mCompanyNo;
        protected int mBranchId;
        protected string mCostCentre;
        protected string mDepartmentCode;
        protected string mLastName;
        protected string mFirstName;
        protected DateTime mDateOfBirth;
        protected bool mDOBConfirmed;
        protected string mGender_ID;
        protected string mIdentityNo;
        protected int mFundCategory_ID;
        protected int mMaritalStatus_ID;
        protected DateTime mDateJoinedCompany;
        protected DateTime mDateJoinedFund;
        protected DateTime mPensionableServiceDate;
        protected DateTime mTranferInDate;
        protected int mNormalRetAge;
        protected DateTime mNormalRetDate;
        protected double mAnnualSalary;
        protected string mPassportNo;
        protected string mTaxNo;
        protected int mTitle_Id;
        protected int mMonthsWaiting;
        protected DateTime mDateSuspended;
        protected int mMonthsSuspended;
        protected DateTime mDateUnsuspended;
        protected DateTime mDateOfExit;
        protected int mIntExitCode;
        protected int mExitCode;
        protected DateTime mChequeReqDateExitCode;
        protected DateTime mEntryPostedDate;
        protected DateTime mExitLetterDate;
        protected int mCompany_ID;
        protected bool mAuthorised;
        protected int mAuthorisedBy;
        protected DateTime mDateAuthorised;
        protected DateTime mDateModified;
        protected int mModifiedBy;
        protected bool mActive;
        protected int mUploadedBy;
        protected DateTime mDateUploaded;
        protected double mStartupMember;
        protected double mStartupEmployer;
        protected double mTotalStartup;
        protected bool mIsDeferred;
        protected string mComments;
        protected DateTime mInterBranchTransferDate;
        protected Guid mmsrepl_tran_version;
        protected string mSplittedRegNo;
        protected int mSplittedID;
        protected int mSplittedCompanyNo;
        protected double mOldNumber;
        protected int mIdentityTypeID;
        protected int mClientTypeID;
        protected string mMobileNo;
        protected string mEmailAddress;
        protected string mLandline;
        protected bool mAvcsTATUS;
        protected int mClientID;
        protected int mFundID;
        protected int mJobTitleID;
        protected bool mIsprocessed;
        protected int mProcessId;
        protected string mMsgflg = "";
        #endregion

        #region properties
        public string Msgflg
        {
            get { return mMsgflg; }
            set { mMsgflg = value; }
        }
        public string PensionNo
        {
            get { return mPensionNo; }
            set { mPensionNo = value; }
        }

        public string EmployeeReferenceNumber
        {
            get { return mEmployeeReferenceNumber; }
            set { mEmployeeReferenceNumber = value; }
        }
        public int CompanyNo
        {
            get { return mCompanyNo; }
            set { mCompanyNo = value; }
        }

        public int BranchId
        {
            get { return mBranchId; }
            set { mBranchId = value; }
        }
        public string CostCentre
        {
            get { return mCostCentre; }
            set { mCostCentre = value; }
        }
        public string DepartmentCode
        {
            get { return mDepartmentCode; }
            set { mDepartmentCode = value; }
        }
        public string LastName
        {
            get { return mLastName; }
            set { mLastName = value; }
        }
        public string FirstName
        {
            get { return mFirstName; }
            set { mFirstName = value; }
        }
        public DateTime DateOfBirth
        {
            get { return mDateOfBirth; }
            set { mDateOfBirth = value; }
        }
        public bool DOBConfirmed
        {
            get { return mDOBConfirmed; }
            set { mDOBConfirmed = value; }
        }
        public string Gender_ID
        {
            get { return mGender_ID; }
            set { mGender_ID = value; }
        }
        public string IdentityNo
        {
            get { return mIdentityNo; }
            set { mIdentityNo = value; }
        }
        public int FundCategory_ID
        {
            get { return mFundCategory_ID; }
            set { mFundCategory_ID = value; }
        }
        public int MaritalStatus_ID
        {
            get { return mMaritalStatus_ID; }
            set { mMaritalStatus_ID = value; }
        }
        public DateTime DateJoinedCompany
        {
            get { return mDateJoinedCompany; }
            set { mDateJoinedCompany = value; }
        }
        public DateTime DateJoinedFund
        {
            get { return mDateJoinedFund; }
            set { mDateJoinedFund = value; }
        }
        public DateTime PensionableServiceDate
        {
            get { return mPensionableServiceDate; }
            set { mPensionableServiceDate = value; }
        }
        public DateTime TranferInDate
        {
            get { return mTranferInDate; }
            set { mTranferInDate = value; }
        }
        public int NormalRetAge
        {
            get { return mNormalRetAge; }
            set { mNormalRetAge = value; }
        }

        public DateTime NormalRetDatemNormalRetDate
        {
            get { return mNormalRetDate; }
            set { mNormalRetDate = value; }
        }
        public double AnnualSalary
        {
            get { return mAnnualSalary; }
            set { mAnnualSalary = value; }
        }
        public string PassportNo
        {
            get { return mPassportNo; }
            set { mPassportNo = value; }
        }
        public string TaxNo
        {
            get { return mTaxNo; }
            set { mTaxNo = value; }
        }
        public int Title_Id
        {
            get { return mTitle_Id; }
            set { mTitle_Id = value; }
        }
        public int MonthsWaiting
        {
            get { return mMonthsWaiting; }
            set { mMonthsWaiting = value; }
        }
        public DateTime DateSuspended
        {
            get { return mDateSuspended; }
            set { mDateSuspended = value; }
        }
        public int MonthsSuspended
        {
            get { return mMonthsSuspended; }
            set { mMonthsSuspended = value; }
        }
        public DateTime DateUnsuspended
        {
            get { return mDateUnsuspended; }
            set { mDateUnsuspended = value; }
        }
        public DateTime DateOfExit
        {
            get { return mDateOfExit; }
            set { mDateOfExit = value; }
        }
        public int IntExitCode
        {
            get { return mIntExitCode; }
            set { mIntExitCode = value; }
        }
        public int ExitCode
        {
            get { return mExitCode; }
            set { mExitCode = value; }
        }
        public DateTime ChequeReqDateExitCode
        {
            get { return mChequeReqDateExitCode; }
            set { mChequeReqDateExitCode = value; }
        }

        public DateTime EntryPostedDate
        {
            get { return mEntryPostedDate; }
            set { mEntryPostedDate = value; }
        }

        public DateTime ExitLetterDate
        {
            get { return mExitLetterDate; }
            set { mExitLetterDate = value; }
        }
        public int Company_ID
        {
            get { return mCompany_ID; }
            set { mCompany_ID = value; }
        }
        public bool Authorised
        {
            get { return mAuthorised; }
            set { mAuthorised = value; }
        }
        public int AuthorisedBy
        {
            get { return mAuthorisedBy; }
            set { mAuthorisedBy = value; }
        }
        public DateTime DateAuthorised
        {
            get { return mDateAuthorised; }
            set { mDateAuthorised = value; }
        }
        public DateTime DateModified
        {
            get { return mDateModified; }
            set { mDateModified = value; }
        }
        public int ModifiedBy
        {
            get { return mModifiedBy; }
            set { mModifiedBy = value; }
        }
        public bool Active
        {
            get { return mActive; }
            set { mActive = value; }
        }
        public int UploadedBy
        {
            get { return mUploadedBy; }
            set { mUploadedBy = value; }
        }
        public DateTime DateUploaded
        {
            get { return mDateUploaded; }
            set { mDateUploaded = value; }
        }
        public double StartupMember
        {
            get { return StartupMember; }
            set { StartupMember = value; }
        }
        public double StartupEmployer
        {
            get { return mStartupEmployer; }
            set { mStartupEmployer = value; }
        }
        public double TotalStartup
        {
            get { return mTotalStartup; }
            set { mTotalStartup = value; }
        }
        public bool IsDeferred
        {
            get { return mIsDeferred; }
            set { mIsDeferred = value; }
        }
        public string Comments
        {
            get { return mComments; }
            set { mComments = value; }
        }
        public DateTime InterBranchTransferDate
        {
            get { return mInterBranchTransferDate; }
            set { mInterBranchTransferDate = value; }
        }
        public Guid msrepl_tran_version
        {
            get { return mmsrepl_tran_version; }
            set { mmsrepl_tran_version = value; }
        }
        public string SplittedRegNo
        {
            get { return mSplittedRegNo; }
            set { mSplittedRegNo = value; }
        }
        public int SplittedID
        {
            get { return mSplittedID; }
            set { mSplittedID = value; }
        }
        public int SplittedCompanyNo
        {
            get { return mSplittedCompanyNo; }
            set { mSplittedCompanyNo = value; }
        }
        public double OldNumber
        {
            get { return mOldNumber; }
            set { mOldNumber = value; }
        }
        public int IdentityTypeID
        {
            get { return mIdentityTypeID; }
            set { mIdentityTypeID = value; }
        }
        public int ClientTypeID
        {
            get { return mClientTypeID; }
            set { mClientTypeID = value; }
        }
        public string MobileNo
        {
            get { return mMobileNo; }
            set { mMobileNo = value; }
        }
        public string EmailAddress
        {
            get { return mEmailAddress; }
            set { mEmailAddress = value; }
        }
        public string Landline
        {
            get { return mLandline; }
            set { mLandline = value; }
        }
        public bool AvcsTATUS
        {
            get { return mAvcsTATUS; }
            set { mAvcsTATUS = value; }
        }
        public int ClientID
        {
            get { return mClientID; }
            set { mClientID = value; }
        }
        public int FundID
        {
            get { return mFundID; }
            set { mFundID = value; }
        }
        public int JobTitleID
        {
            get { return mJobTitleID; }
            set { mJobTitleID = value; }
        }
        public bool Isprocessed
        {
            get { return mIsprocessed; }
            set { mIsprocessed = value; }
        }
        public int ProcessId
        {
            get { return mProcessId; }
            set { mProcessId = value; }
        }
        #endregion
        public SyncingMemberUploads(string conName, int liveConn)
        {
            mConString = conName;
            //mLiveConString = liveCon;
            portalDB = new DatabaseProviderFactory().Create(conName);
            liveDB = new DatabaseProviderFactory().Create("penAdmin");
            //OpenConnection();

        }


        #region Methods
        public DataSet GetMemberUploads(int Isprocessed)
        {
            try
            {
                System.Data.Common.DbCommand cmd = liveDB.GetStoredProcCommand("sp_Select_MemberUploads");
                liveDB.AddInParameter(cmd, "@Isprocessed", DbType.Int32, Isprocessed);
                DataSet ds = liveDB.ExecuteDataSet(cmd);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    return ds;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                mMsgflg = ex.Message;
                LogScriptor.WriteErrorLog(mMsgflg);
                return null;
            }
        }
        #endregion
    }
}
