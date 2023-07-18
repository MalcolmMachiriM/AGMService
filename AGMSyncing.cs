using Microsoft.Practices.EnterpriseLibrary.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AGMService
{
    #region enums
    public enum LookUp
    {
        AGMRegisstration = 1,
        AdminUpdated = 2,
        AdminResponse = 3,
        AGMUpdate = 4
    }

    #endregion
    public class AGMSyncing
    {
        #region variables
        private MySqlConnection cnxn;
        private string server;
        private string database;
        private string uid;
        private string password;
        protected long mEventID;
        protected long mRegistrationID;
        protected string mQuery;
        protected Database portalDB;
        protected Database liveDB;
        protected string mConString;
        protected string mLiveConString;
        protected string mMsgflg = "";

        protected int mID;
        protected int mPortalID;
        protected int mPensionNo;
        protected string mDescription;
        protected string mCity;
        protected string mQueryType;
        protected string mContentType;
        protected byte mData;
        protected DateTime mDateCreated;
        protected bool misSolved;
        protected string mSubject;
        protected string mRegNo;
        protected string mType;
        protected string mComment;
        protected string mActionType;
        protected string mDocumentName;
        protected int mSyncID;
        //member uploads vars

        #endregion
        #region Properties

        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; }
        }
        public int PortalID
        {
            get { return mPortalID; }
            set { mPortalID = value; }
        }
        public string City
        {
            get { return mCity; }
            set { mCity = value; }
        }
        public string QueryType
        {
            get { return mQueryType; }
            set { mQueryType = value; }
        }
        public string ContentType
        {
            get { return mContentType; }
            set { mContentType = value; }
        }
        public byte Data
        {
            get { return mData; }
            set { mData = value; }
        }
        public bool isSolved
        {
            get { return misSolved; }
            set { misSolved = value; }
        }
        public string Subject
        {
            get { return mSubject; }
            set { mSubject = value; }
        }
        public string RegNo
        {
            get { return mRegNo; }
            set { mRegNo = value; }
        }
        public string Type
        {
            get { return mType; }
            set { mType = value; }
        }
        public string Comment
        {
            get { return mComment; }
            set { mComment = value; }
        }
        public string ActionType
        {
            get { return mActionType; }
            set { mActionType = value; }
        }
        public string DocumentName
        {
            get { return mDocumentName; }
            set { mDocumentName = value; }
        }
        public int SyncID
        {
            get { return mSyncID; }
            set { mSyncID = value; }
        }
        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }

        public int PensionNo
        {
            get { return mPensionNo; }
            set { mPensionNo = value; }
        }

        public long RegistrationID
        {
            get { return mRegistrationID; }
            set { mRegistrationID = value; }
        }

        public DateTime DateCreated
        {
            get { return mDateCreated; }
            set { mDateCreated = value; }
        }

        
        public string Query
        {
            get { return mQuery; }
            set { mQuery = value; }
        }

       
        public string Msgflg
        {
            get { return mMsgflg; }
            set { mMsgflg = value; }
        }

        public string LiveConString
        {
            get { return mLiveConString; }
        }

        public string ConString
        {
            get { return mConString; }
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

        public AGMSyncing(string ConName, int liveCon)
        {
            mConString = ConName;
            //mLiveConString = liveCon;
            portalDB = new DatabaseProviderFactory().Create(ConName);
            liveDB = new DatabaseProviderFactory().Create("penAdmin");
            //OpenConnection();
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
        public DataSet getQueryFromAgmPortal(int SyncID)
        {
            try
            {
                System.Data.Common.DbCommand cmd = portalDB.GetStoredProcCommand("sp_Select_PoralQueries");
                //System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("Get_Participarting_Employer_Portal");
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

            System.Data.Common.DbCommand cmd = liveDB.GetStoredProcCommand("sp_Save_PortalQueries");

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

            System.Data.Common.DbCommand cmd = portalDB.GetStoredProcCommand("sp_Save_PortalQueries");

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


            System.Data.Common.DbCommand cmd = liveDB.GetStoredProcCommand("sp_Update_PortalQueries_Status");
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
            System.Data.Common.DbCommand cmd = portalDB.GetStoredProcCommand("sp_Update_PortalQueries_Status");
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
        protected internal virtual void LoadDataRecord(DataRow rw)
        {


            mID = ((object)rw["ID"] == DBNull.Value) ? 0 : int.Parse(rw["ID"].ToString());
            mPensionNo = ((object)rw["PensionNo"] == DBNull.Value) ? 0 : int.Parse(rw["PensionNo"].ToString());
            mRegistrationID = ((object)rw["RegistrationID"] == DBNull.Value) ? 0 : int.Parse(rw["RegistrationID"].ToString()); 
            mDateCreated = (rw["DateCreated"] == DBNull.Value) ? DateTime.MinValue : (DateTime)rw["DateCreated"];
            misSolved = ((object)rw["isSolved"] == DBNull.Value) ? false : bool.Parse(rw["isSolved"].ToString());
            mQuery = ((object)rw["Query"] == DBNull.Value) ? "" : rw["Query"].ToString();
            mQueryType = ((object)rw["QueryType"] == DBNull.Value) ? "" : rw["QueryType"].ToString();
            mComment = ((object)rw["Comment"] == DBNull.Value) ? "" : rw["Comment"].ToString();
            mActionType = ((object)rw["ActionType"] == DBNull.Value) ? "" : rw["ActionType"].ToString();
            mSyncID = (int)(((object)rw["SyncID"] == DBNull.Value) ? "" : rw["SyncID"]);
            mDescription = ((object)rw["Description"] == DBNull.Value) ? "" : rw["Description"].ToString();


        }

        public virtual void GenerateSaveParameters(ref Database db, ref System.Data.Common.DbCommand cmd)
        {
            db.AddInParameter(cmd, "@ID", DbType.Int32, mID);
            db.AddInParameter(cmd, "@PensionNo", DbType.Int32, mPensionNo);
            db.AddInParameter(cmd, "@Description", DbType.String, mDescription);
            db.AddInParameter(cmd, "@City", DbType.String, mCity);
            db.AddInParameter(cmd, "@QueryType", DbType.String, mQueryType);
            db.AddInParameter(cmd, "@ContentType", DbType.String, mContentType);
            db.AddInParameter(cmd, "@Data", DbType.Binary, mData);
            db.AddInParameter(cmd, "@isSolved", DbType.Boolean, misSolved);
            db.AddInParameter(cmd, "@Subject", DbType.String, mSubject);
            db.AddInParameter(cmd, "@RegNo", DbType.String, mRegNo);
            db.AddInParameter(cmd, "@Type", DbType.String, mType);
            db.AddInParameter(cmd, "@Comment", DbType.String, mComment);
            db.AddInParameter(cmd, "@ActionType", DbType.String, mActionType);
            db.AddInParameter(cmd, "@DocumentName", DbType.String, mDocumentName);
            db.AddInParameter(cmd, "@SyncID", DbType.Int32, mSyncID);
            db.AddInParameter(cmd, "@PortalID", DbType.Int32, mPortalID);
        }

        #endregion
    }
}
