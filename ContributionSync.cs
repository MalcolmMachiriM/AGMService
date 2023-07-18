using Microsoft.Practices.EnterpriseLibrary.Data;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Pkcs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Design.CommonDesignTime;

namespace AGMService
{
    public enum ContributionsUploads
    {
        AddedToPortal = 1,
        AdminUpdated = 2,
        AdminResponse = 3,
        PortalUpdate = 4

    }
    public class ContributionSync
    {
        #region variables

        protected string mMsgflg = "";
        private MySqlConnection cnxn;
        protected Database portalDB;
        protected Database liveDB;
        protected string mConString;
        protected string mLiveConString;
        protected int mID;


        private string server;
        private string database;
        private string uid;
        private string password;

        protected int mMemberID;
        protected string mNationalID;
        protected string mPeriod;
        protected float mSalary;
        protected string mRegNo;
        protected DateTime mPaymentDate;
        protected float mXContribution;
        protected float mYContribution;
        protected float mZContribution;
        protected float mGrossYContribution;
        protected float mExpenses;
        protected float mExpectedX;
        protected float mExpectedY;
        protected int mUserID;
        protected DateTime mDateCaptured;
        protected Guid mTransID;
        protected Guid mBatchID;
        protected float mTransferInMember;
        protected float mTransferInEmployer;
        protected float mOtherMember;
        protected float mOtherEmployer;
        protected float mTotal;
        protected bool misStartup;
        protected double mBackPay;
        protected string mBranchCode;
        protected DateTime mLatestUpdateDate;
        protected bool mIsHistory;
        protected DateTime mPeriodDate;
        protected string mMyKey;
        protected int mCompanyID;
        protected string mSplittedRegNo;
        protected int mSplittedMemberID;
        protected int mSplittedBrachCode;
        protected DateTime mDateSplitted;
        protected int mBonusTypeID;
        protected string mBatchNo;
        protected string mPlatform;
        protected int mSyncID;
        protected int mPortalID;
        #endregion

        #region properties
        public string Msgflg
        {
            get { return mMsgflg; }
            set { mMsgflg = value; }
        }
        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }
        public int PortalID
        {
            get { return mPortalID; }
            set { mPortalID = value; }
        }
        public int SyncID
        {
            get { return mSyncID; }
            set { mSyncID = value; }
        }
        public string RegNo
        {
            get { return mRegNo; }
            set { mRegNo = value; }
        }
        public int MemberID
        {
            get { return mMemberID; }
            set { mMemberID = value; }
        }
        public string NationalID
        {
            get { return mNationalID; }
            set { mNationalID = value; }
        }
        public string Period
        {
            get { return mPeriod; }
            set { mPeriod = value; }
        }

        public float Salary
        {
            get { return mSalary; }
            set { mSalary = value; }
        }
        public DateTime PaymentDate
        {
            get { return mPaymentDate; }
            set { mPaymentDate = value; }
        }

        public float XContribution
        {
            get { return mXContribution; }
            set { mXContribution = value; }
        }
        public float YContribution
        {
            get { return mYContribution; }
            set { mYContribution = value; }
        }
        public float ZContribution
        {
            get { return mZContribution; }
            set { mZContribution = value; }
        }
        public float GrossYContribution
        {
            get { return mGrossYContribution; }
            set { mGrossYContribution = value; }
        }
        public float Expenses
        {
            get { return mExpenses; }
            set { mExpenses = value; }
        }
        public float ExpectedX
        {
            get { return mExpectedX; }
            set { mExpectedX = value; }
        }
        public float ExpectedY
        {
            get { return mExpectedY; }
            set { mExpectedY = value; }
        }
        public int UserID
        {
            get { return mUserID; }
            set { mUserID = value; }
        }
        public DateTime DateCaptured
        {
            get { return mDateCaptured; }
            set { mDateCaptured = value; }
        }
        public Guid TransID
        {
            get { return mTransID; }
            set { mTransID = value; }
        }
        public Guid BatchID
        {
            get { return mBatchID; }
            set { mBatchID = value; }
        }
        public float TransferInMember
        {
            get { return mTransferInMember; }
            set { mTransferInMember = value; }
        }
        public float TransferInEmployer
        {
            get { return mTransferInEmployer; }
            set { mTransferInEmployer = value; }
        }
        public float OtherMember
        {
            get { return mOtherMember; }
            set { mOtherMember = value; }
        }
        public float OtherEmployer
        {
            get { return mOtherEmployer; }
            set { mOtherEmployer = value; }
        }
        public float Total
        {
            get { return mTotal; }
            set { mTotal = value; }
        }
        public bool isStartup
        {
            get { return misStartup; }
            set { misStartup = value; }
        }

        public double BackPay
        {
            get { return mBackPay; }
            set { mBackPay = value; }
        }
        public string BranchCode
        {
            get { return mBranchCode; }
            set { mBranchCode = value; }
        }
        public DateTime LatestUpdateDate
        {
            get { return mLatestUpdateDate; }
            set { mLatestUpdateDate = value; }
        }
        public bool IsHistory
        {
            get { return mIsHistory; }
            set { mIsHistory = value; }
        }
        public DateTime PeriodDate
        {
            get { return mPeriodDate; }
            set { mPeriodDate = value; }
        }
        public string MyKey
        {
            get { return mMyKey; }
            set { mMyKey = value; }
        }
        public int CompanyID
        {
            get { return mCompanyID; }
            set { mCompanyID = value; }
        }
        public string SplittedRegNo
        {
            get { return mSplittedRegNo; }
            set { mSplittedRegNo = value; }
        }
        public int SplittedMemberID
        {
            get { return mSplittedMemberID; }
            set { mSplittedMemberID = value; }
        }
        public int SplittedBrachCode
        {
            get { return mSplittedBrachCode; }
            set { mSplittedBrachCode = value; }
        }
        public DateTime DateSplitted
        {
            get { return mDateSplitted; }
            set { mDateSplitted = value; }
        }
        public int BonusTypeID
        {
            get { return mBonusTypeID; }
            set { mBonusTypeID = value; }
        }
        public string BatchNo
        {
            get { return mBatchNo; }
            set { mBatchNo = value; }
        }

        public string Platform
        {
            get { return mPlatform; }
            set { mPlatform = value; }
        }

        public double StartupMember
        {
            get { return StartupMember; }
            set { StartupMember = value; }
        }
       
        #endregion

        #region Constructor
        public ContributionSync(string conName, int liveConn)
        {
            mConString = conName;
            //mLiveConString = liveCon;
            portalDB = new DatabaseProviderFactory().Create(conName);
            liveDB = new DatabaseProviderFactory().Create("penAdmin");
            //OpenConnection();

        }
        #endregion


        #region methods
        private bool OpenConnection()
        {
            try
            {
                cnxn.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        LogScriptor.WriteErrorLog("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        LogScriptor.WriteErrorLog("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }
        //Close connection
        private bool CloseConnection()
        {
            try
            {
                cnxn.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                LogScriptor.WriteErrorLog(ex.Message);
                return false;
            }
        }


        private void connectToMySQL()
        {
            server = "";
            database = "PensionsManComarton";
            uid = "root";
            password = "";
            string connectionString;
            connectionString = "SERVER=" + server + "; DATABASE=" + database + "; UID" + uid + "PASSWORD=" + password + "; Convert Zero DateTime=True; default command timeout = 120;";
            cnxn = new MySqlConnection(connectionString);

        }

       

        protected DataSet ReturnDs(string str)
        {

            try
            {
                DataSet ds = new DataSet();
                ds = portalDB.ExecuteDataSet(CommandType.Text, str);
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
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

                LogScriptor.WriteErrorLog("An Error occured: " + ex.Message);
                return null;
            }

        }
        public DataSet getContributionsFromPortal(int SyncID)
        {
            try
            {
                System.Data.Common.DbCommand cmd = portalDB.GetStoredProcCommand("sp_Select_PortalContributions");
                portalDB.AddInParameter(cmd, "@SyncID", DbType.Int32, SyncID);
                DataSet ds = portalDB.ExecuteDataSet(cmd);


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
                LogScriptor.WriteErrorLog(ex.Message);
                return null;
            }
        }

        public virtual bool SavetoAdmin()
        {
            //Posting New Queries to The Admin DB

            System.Data.Common.DbCommand cmd = liveDB.GetStoredProcCommand("ContributionsPortal_ins_Service");

            GenerateSaveParameters(ref liveDB, ref cmd);


            try
            {
                DataSet ds = liveDB.ExecuteDataSet(cmd);


                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    mID = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());

                }

                return true;


            }
            catch (Exception ex)
            {
                mMsgflg = ex.Message;
                return false;

            }

        }

        public virtual bool SavePortalUpdate()
        {
            //Posting New Queries to The Admin DB

            System.Data.Common.DbCommand cmd = portalDB.GetStoredProcCommand("ContributionsPortal_ins_Service");

            GenerateSaveParameters(ref portalDB, ref cmd);


            try
            {
                DataSet ds = portalDB.ExecuteDataSet(cmd);


                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    mID = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());

                }

                return true;


            }
            catch (Exception ex)
            {
                mMsgflg = ex.Message;
                return false;

            }

        }

        public DataSet getQueryFromAdmin(int SyncID)
        {
            try
            {
                System.Data.Common.DbCommand cmd = liveDB.GetStoredProcCommand("sp_Select_PoralQueries");
                //System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("Get_Participarting_Employer_Portal");
                liveDB.AddInParameter(cmd, "@SyncID", DbType.Int32, SyncID);
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


        public bool UpdateLiveDB(int QueryID, int StatusID)
        {


            System.Data.Common.DbCommand cmd = liveDB.GetStoredProcCommand("sp_Update_PortalContributions_Status");
            liveDB.AddInParameter(cmd, "@PortalID", DbType.Int32, QueryID);
            liveDB.AddInParameter(cmd, "@SyncID", DbType.Int32, StatusID);



            try
            {
                DataSet ds = liveDB.ExecuteDataSet(cmd);


                return true;


            }
            catch (Exception ex)
            {
                mMsgflg = ex.Message;
                return false;

            }

        }

        public bool UpdateFromAdmin(int QueryID, int StatusID)
        {
            //System.Data.Common.DbCommand cmd = portalDB.GetStoredProcCommand("sp_Sync_AGMQueries");
            System.Data.Common.DbCommand cmd = portalDB.GetStoredProcCommand("sp_Update_PortalContributions_Status");
            portalDB.AddInParameter(cmd, "@PortalID", DbType.Int32, QueryID);
            portalDB.AddInParameter(cmd, "@SyncID", DbType.Int32, StatusID);

            try
            {
                DataSet ds = portalDB.ExecuteDataSet(cmd);

                return true;

            }
            catch (Exception ex)
            {
                mMsgflg = ex.Message;
                return false;

            }

        }
        //protected internal virtual void LoadDataRecord(DataRow rw)
        //{


        //    mID = ((object)rw["ID"] == DBNull.Value) ? 0 : int.Parse(rw["ID"].ToString());
        //    mPensionNo = ((object)rw["PensionNo"] == DBNull.Value) ? 0 : int.Parse(rw["PensionNo"].ToString());
        //    mRegistrationID = ((object)rw["RegistrationID"] == DBNull.Value) ? 0 : int.Parse(rw["RegistrationID"].ToString());
        //    mDateCreated = (rw["DateCreated"] == DBNull.Value) ? DateTime.MinValue : (DateTime)rw["DateCreated"];
        //    misSolved = ((object)rw["isSolved"] == DBNull.Value) ? false : bool.Parse(rw["isSolved"].ToString());
        //    mQuery = ((object)rw["Query"] == DBNull.Value) ? "" : rw["Query"].ToString();
        //    mQueryType = ((object)rw["QueryType"] == DBNull.Value) ? "" : rw["QueryType"].ToString();
        //    mComment = ((object)rw["Comment"] == DBNull.Value) ? "" : rw["Comment"].ToString();
        //    mActionType = ((object)rw["ActionType"] == DBNull.Value) ? "" : rw["ActionType"].ToString();
        //    mSyncID = (int)(((object)rw["SyncID"] == DBNull.Value) ? "" : rw["SyncID"]);
        //    mDescription = ((object)rw["Description"] == DBNull.Value) ? "" : rw["Description"].ToString();


        //}

        public virtual void GenerateSaveParameters(ref Database db, ref System.Data.Common.DbCommand cmd)
        {
            db.AddInParameter(cmd, "@ID", DbType.Int32, mID);
            db.AddInParameter(cmd, "@MemberID", DbType.Int32, mMemberID);
            db.AddInParameter(cmd, "@NationalID", DbType.String, mNationalID);
            db.AddInParameter(cmd, "@Period", DbType.String, mPeriod);
            db.AddInParameter(cmd, "@Salary", DbType.Decimal, mSalary);
            db.AddInParameter(cmd, "@RegNo", DbType.String, mRegNo);
            db.AddInParameter(cmd, "@PaymentDate", DbType.DateTime, mPaymentDate);
            db.AddInParameter(cmd, "@XContribution", DbType.Decimal, mXContribution);
            db.AddInParameter(cmd, "@YContribution", DbType.Decimal, mYContribution);
            db.AddInParameter(cmd, "@ZContribution", DbType.Decimal, mZContribution);
            db.AddInParameter(cmd, "@GrossYContribution", DbType.Decimal, mGrossYContribution);
            db.AddInParameter(cmd, "@Expenses", DbType.Decimal, mExpenses);
            db.AddInParameter(cmd, "@ExpectedX", DbType.Decimal, mExpectedX);
            db.AddInParameter(cmd, "@ExpectedY", DbType.Decimal, mExpectedY);
            db.AddInParameter(cmd, "@UserID", DbType.Int32, mUserID);
            db.AddInParameter(cmd, "@DateCaptured", DbType.DateTime, mDateCaptured);
            db.AddInParameter(cmd, "@TransID", DbType.Guid, mTransID);
            db.AddInParameter(cmd, "@BatchID", DbType.Guid, mBatchID);
            db.AddInParameter(cmd, "@TransferInMember", DbType.Decimal, mTransferInMember);
            db.AddInParameter(cmd, "@TransferInEmployer", DbType.Decimal, mTransferInEmployer);
            db.AddInParameter(cmd, "@OtherMember", DbType.Decimal, mOtherMember);
            db.AddInParameter(cmd, "@OtherEmployer", DbType.Decimal, mOtherEmployer);
            db.AddInParameter(cmd, "@Total", DbType.Decimal, mTotal);
            db.AddInParameter(cmd, "@isStartup", DbType.Boolean, misStartup);
            db.AddInParameter(cmd, "@BackPay", DbType.Double, mBackPay);
            db.AddInParameter(cmd, "@BranchCode", DbType.String, mBranchCode);
            db.AddInParameter(cmd, "@LatestUpdateDate", DbType.Decimal, mLatestUpdateDate);
            db.AddInParameter(cmd, "@IsHistory", DbType.Boolean, mIsHistory);
            db.AddInParameter(cmd, "@PeriodDate", DbType.DateTime, mPeriodDate);
            db.AddInParameter(cmd, "@MyKey", DbType.String, mMyKey);
            db.AddInParameter(cmd, "@CompanyID", DbType.Int32, mCompanyID);
            db.AddInParameter(cmd, "@SplittedRegNo", DbType.String, mSplittedRegNo);
            db.AddInParameter(cmd, "@SplittedMemberID", DbType.Int32, mSplittedMemberID);
            db.AddInParameter(cmd, "@SplittedBrachCode", DbType.Int32, mSplittedBrachCode);
            db.AddInParameter(cmd, "@DateSplitted", DbType.DateTime, mDateSplitted);
            db.AddInParameter(cmd, "@BonusTypeID", DbType.Int32, mBonusTypeID);
            db.AddInParameter(cmd, "@BatchNo", DbType.String, mBatchNo);
            db.AddInParameter(cmd, "@Platform", DbType.String, mPlatform);
            db.AddInParameter(cmd, "@SyncID", DbType.Int32, mSyncID);
            db.AddInParameter(cmd, "@PortalID", DbType.Int32, mPortalID);
        }

        #endregion
    }
}
