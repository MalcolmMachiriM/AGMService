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
    public enum LookUp
    {
        AGMRegisstration = 1,
        AdminUpdated = 2,
        AdminResponse = 3,
        AGMUpdate = 4
    }
    public class AGMSyncing
    {
        #region variables
        private MySqlConnection cnxn;
        private string server;
        private string database;
        private string uid;
        private string password;
        protected long mID;
        protected long mEventID;
        protected long mRegistrationID;
        protected string mDateCreated;
        protected bool misSolved;
        protected string mQuery;
        protected string mQueryType;
        protected string mComment;
        protected string mActionType;
        protected int mSyncID;

        protected Database portalDB;
        protected Database liveDB;
        protected string mConString;
        protected string mLiveConString;
        protected int mPortalID;
        protected string mMsgflg = "";

        #endregion
        #region Properties
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
        public long ID
        {
            get { return mID; }
            set { mID = value; }
        }

        public long EventID
        {
            get { return mEventID; }
            set { mEventID = value; }
        }

        public long RegistrationID
        {
            get { return mRegistrationID; }
            set { mRegistrationID = value; }
        }

        public string DateCreated
        {
            get { return mDateCreated; }
            set { mDateCreated = value; }
        }

        public bool isSolved
        {
            get { return misSolved; }
            set { misSolved = value; }
        }

        public string Query
        {
            get { return mQuery; }
            set { mQuery = value; }
        }

        public string QueryType
        {
            get { return mQueryType; }
            set { mQueryType = value; }
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
                System.Data.Common.DbCommand cmd = portalDB.GetStoredProcCommand("sp_Select_AGMQueries");
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

            System.Data.Common.DbCommand cmd = liveDB.GetStoredProcCommand("sp_Sync_AGMQueries");

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

            System.Data.Common.DbCommand cmd = portalDB.GetStoredProcCommand("sp_Sync_AGMQueries");

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
                System.Data.Common.DbCommand cmd = liveDB.GetStoredProcCommand("sp_Select_AGMQueries");
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


            System.Data.Common.DbCommand cmd = liveDB.GetStoredProcCommand("sp_Update_AGMQueries_Status");
            liveDB.AddInParameter(cmd, "@QueryID", DbType.Int32, QueryID);
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
            System.Data.Common.DbCommand cmd = portalDB.GetStoredProcCommand("sp_Update_AGMQueries_Status");
            portalDB.AddInParameter(cmd, "@QueryID", DbType.Int32, QueryID);
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
            mEventID = ((object)rw["EventID"] == DBNull.Value) ? 0 : int.Parse(rw["EventID"].ToString());
            mRegistrationID = ((object)rw["RegistrationID"] == DBNull.Value) ? 0 : int.Parse(rw["RegistrationID"].ToString());
            mDateCreated = ((object)rw["DateCreated"] == DBNull.Value) ? "" : rw["DateCreated"].ToString();
            misSolved = ((object)rw["isSolved"] == DBNull.Value) ? false : bool.Parse(rw["isSolved"].ToString());
            mQuery = ((object)rw["Query"] == DBNull.Value) ? "" : rw["Query"].ToString();
            mQueryType = ((object)rw["QueryType"] == DBNull.Value) ? "" : rw["QueryType"].ToString();
            mComment = ((object)rw["Comment"] == DBNull.Value) ? "" : rw["Comment"].ToString();
            mActionType = ((object)rw["ActionType"] == DBNull.Value) ? "" : rw["ActionType"].ToString();
            mSyncID = (int)(((object)rw["SyncID"] == DBNull.Value) ? "" : rw["SyncID"]);


        }

        public virtual void GenerateSaveParameters(ref Database db, ref System.Data.Common.DbCommand cmd)
        {
            db.AddInParameter(cmd, "@ID", DbType.Int32, mID);
            db.AddInParameter(cmd, "@EventID", DbType.Int32, mEventID);
            db.AddInParameter(cmd, "@RegistrationID", DbType.Int32, mRegistrationID);
            //db.AddInParameter(cmd, "@DateCreated", DbType.String, mDateCreated);
            db.AddInParameter(cmd, "@isSolved", DbType.Boolean, misSolved);
            db.AddInParameter(cmd, "@Query", DbType.String, mQuery);
            db.AddInParameter(cmd, "@QueryType", DbType.String, mQueryType);
            db.AddInParameter(cmd, "@Comment", DbType.String, mComment);
            db.AddInParameter(cmd, "@ActionType", DbType.String, mActionType);
            db.AddInParameter(cmd, "@SyncID", DbType.Int32, mSyncID);
            db.AddInParameter(cmd, "@PortalID", DbType.Int32, mPortalID);
        }

        #endregion
    }
}
