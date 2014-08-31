/*---------------------------------------------------------------------------
--                              rtdViewer                                  -- 
-----------------------------------------------------------------------------
--                                                                         --
--                                                                         --
--  author:   Álvaro García                                                --
--  website:  www.binarynonsense.com                                       --
--                                                                         --
--  file: data.cs                                                          --
--                                                                         --
--                                                                         --
-----------------------------------------------------------------------------
--                      last update: 29 Jan 2009                           --
---------------------------------------------------------------------------*/

//reader

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Gtk;
using System.Data;
using Mono.Data.SqliteClient;

namespace AGC
{


	 
	public static class DataFunctions{
	
	        //MEMBERS
	        
	        public static string version = "0.57 - Beta";
	        public static string lastUpdate = "29 Jan 2009";
	        
	        public static string functionID = "1";//the function we want the data
	        public static string functionXrefID="1";
	        public static string fileSource = "Not Available";
	        public static string fileSourceID = "1";
	        public static string functionSource = "Not Available";
	        
	        public static int sourceInitLine;
                public static int sourceEndLine;
	        
	        public static int transitionSourceInitLine;
	        public static int transitionSourceEndLine;
	
	        public static string dataBaseFileName = "example.rtd";
	        public static string functionName = "Function Name";
	        //Dictionary:
        	//http://msdn.microsoft.com/en-us/library/xfhwa508.aspx
	        public static Dictionary<string, string> ids2Names = new Dictionary<string, string>();
        	public static ArrayList validIDs = new ArrayList();
        	public static double clock_freq = 0;
        	
        	public static double unitNanoSec = 1E-9;        	
        	public static string unitStringNanoSec = "nanoseconds";
        	public static string unitStringShortNanoSec = "ns";
        	
        	public static double unitMicroSec = 1E-6;        	
        	public static string unitStringMicroSec = "microseconds";
        	public static string unitStringShortMicroSec = "µs";
        	
        	public static double unitMiliSec = 1E-3;
        	public static string unitStringMiliSec = "miliseconds";
        	public static string unitStringShortMiliSec = "ms";
        	
        	public static double unitSec = 1;
        	public static string unitStringSec = "seconds";
        	public static string unitStringShortSec = "s";
        	
        	public static double unit = unitMicroSec;
        	public static string unitString = unitStringMicroSec;
        	public static string unitStringShort = unitStringShortMicroSec;
        	
        	public static double factor = 1;
        	public static string rapitimeVersion = "0.0";
        	
        	public static double met_max = 0.0;
		public static double met_min = 0.0;
		public static double self_met_total = 0.0;
		public static double self_wcet = 0.0;
		public static double tests = 0.0;			
		public static double sub_met_total = 0.0;
		public static double sub_wcet = 0.0;		
		public static double met_avg = 0.0;
		
		public static ArrayList endToEnd = new ArrayList();
		
		public static IDbConnection dbcon;
		
	        /*public struct TransitionDataStruct{
		
			public int initLine;
			public int endLine;
			//public int xrefID;
			public double wcet;
	
		}*/
		public struct TransitionDataStruct{
		
		        public int xrefID;
			public int initLine;
			public int endLine;
			public double wcet;
	
		}
		
		public static TransitionDataStruct transitionData;
		//public static Dictionary<int, TransitionDataStruct> transitionsData = new Dictionary<int, TransitionDataStruct>();
	        public static ArrayList transitionsData = new ArrayList();
	        
	        
	        public struct ColorsStruct{		
		       
			public double r1;
			public double g1;
			public double b1;
			
			public double r2;
			public double g2;
			public double b2;
			
			public ColorsStruct(double x1,double y1,double z1,double x2,double y2,double z2){
			        r1=x1;
			        g1=y1;
			        b1=z1;
			        r2=x2;
			        g2=y2;
			        b2=z2;
			}
	
		}
		public static ColorsStruct graphColors = new ColorsStruct(0.27,0.42,0.96,0.05,0.23,0.87);
		
	        
	        //METHODS ///////////////////////////////////////////////////////////
	        /////////////////////////////////////////////////////////////////////
	        
	        public static void DBOpen(){
	        
	                string connectionString = "URI=file:"+dataBaseFileName+",version=3";                            
        	        
        		dbcon = (IDbConnection) new SqliteConnection(connectionString);
        		dbcon.Open();
	        
	        }//end DBOpen
	        
	        public static void DBClose(){
	        
	                dbcon.Close();
        		dbcon = null;
	        
	        }//end DBOpen
	        
	        public static void DBExtractFreq(){
	        
	                IDbCommand dbcmd = dbcon.CreateCommand();
        		IDataReader reader;
        		string sql;
        		
        		////////////CLOCK FREQ//////////////           		
        		
        		sql =
        		"SELECT value " +
        		"FROM Info WHERE name='clock_freq'";
        		dbcmd.CommandText = sql;
        		reader = dbcmd.ExecuteReader();
        		reader.Read();
        		clock_freq = Convert.ToDouble(reader.GetString (0));	
        		factor=clock_freq*unit;
        		
        		// clean up
        		reader.Close();
        		reader = null;
        		dbcmd.Dispose();
        		dbcmd = null;
	        
	        }//end DBExtractFreq
	        
	        public static void DBExtractVersion(){
	        
	                IDbCommand dbcmd = dbcon.CreateCommand();
        		IDataReader reader;
        		string sql;
        		
        		////////////RT VERSION//////////////           		
        		
        		sql =
        		"SELECT value " +
        		"FROM Info WHERE name='tool_version'";
        		dbcmd.CommandText = sql;
        		reader = dbcmd.ExecuteReader();
        		reader.Read();
        		rapitimeVersion = reader.GetString (0);
        		// clean up
        		reader.Close();
        		reader = null;
        		dbcmd.Dispose();
        		dbcmd = null;
	        
	        }//end DBExtractVersion
	        
	        
	        public static void DBExtractContext(){
	        
	                IDbCommand dbcmd = dbcon.CreateCommand();
        		IDataReader reader;
        		string sql;
        		/////////////CONTEXT//////////////////// 		

        		sql = "SELECT id, displayName, uniqueName FROM Subprogram";
        		dbcmd.CommandText = sql;
        		reader = dbcmd.ExecuteReader();  
        		
        		while(reader.Read()) {

        			if (reader.GetString (1)!=""){
        			        ids2Names.Add(reader.GetString (0),reader.GetString (1));
        			}else{
        			        ids2Names.Add(reader.GetString (0),reader.GetString (2));
        			}
        		}
        		
        		functionName = ids2Names[functionID];
        		
        		// clean up
        		reader.Close();
        		reader = null;
        		dbcmd.Dispose();
        		dbcmd = null;
	        
	        }//end DBExtractContext;
	        
	        public static void DBExtractValidIDs(){
	        
	                IDbCommand dbcmd = dbcon.CreateCommand();
        		IDataReader reader;
        		string sql;
        		/////////////////////SAVE VALID IDs
        		
        		sql =
        		"SELECT id FROM Context WHERE met_max IS NOT NULL";
        		dbcmd.CommandText = sql;
        		reader = dbcmd.ExecuteReader();
        		
        		while(reader.Read()) {

        		        validIDs.Add(reader.GetString (0));

        		}
        		
        		// clean up
        		reader.Close();
        		reader = null;
        		dbcmd.Dispose();
        		dbcmd = null;
	        
	        }//end DBExtractValidIDs
	        
	        public static void DBExtractFunctionData(){
	        
	                IDbCommand dbcmd = dbcon.CreateCommand();
        		IDataReader reader;
        		string sql;
        		/////////////////////SAVE FUNCTION TIME DATA
        		
        		sql =
        		"SELECT met_max, met_min, self_met_total, self_wcet, tests, sub_met_total, sub_wcet FROM Context WHERE id=" + functionID;
        		dbcmd.CommandText = sql;
        		reader = dbcmd.ExecuteReader();
        		
        		while(reader.Read()) {

                                met_max = Convert.ToDouble(reader.GetString (0))/factor;
        			met_min = Convert.ToDouble(reader.GetString (1))/factor;
        			self_met_total = Convert.ToDouble(reader.GetString (2))/factor;
        			self_wcet = Convert.ToDouble(reader.GetString (3))/factor;
        			tests = Convert.ToDouble(reader.GetString (4));			
        			sub_met_total = Convert.ToDouble(reader.GetString (5))/factor;
        			sub_wcet = Convert.ToDouble(reader.GetString (6))/factor;
        			met_avg = ((self_met_total+sub_met_total)/tests);

        		}
        		
        		// clean up
        		reader.Close();
        		reader = null;
        		dbcmd.Dispose();
        		dbcmd = null;
	        
	        }//end DBExtractFunctionData
	        
	        public static void DBExtractEndToEndData(){
	        
	                IDbCommand dbcmd = dbcon.CreateCommand();
        		IDataReader reader;
        		
        		endToEnd.Clear();
        		endToEnd.TrimToSize ();
        		
        		string sql;
        		////////////////////END TO END
              
		        sql =
                	"SELECT  self, sub " +
                	"FROM EndToEndRecord WHERE context_id =" + functionID;
                	dbcmd.CommandText = sql;
                	reader = dbcmd.ExecuteReader();
                	while(reader.Read()) {

        			double self = Convert.ToDouble(reader.GetString (0))/factor;
        			double sub = Convert.ToDouble(reader.GetString (1))/factor;
        			endToEnd.Add(self+sub);

		        }
        		
        		// clean up
        		reader.Close();
        		reader = null;
        		dbcmd.Dispose();
        		dbcmd = null;
	        
	        }//end DBExtractEndToEndData
	        
	        public static void DBExtractSourceData(){
	        
	                //Console.WriteLine(functionID);
	        
	                IDbCommand dbcmd = dbcon.CreateCommand();
        		IDataReader reader;
        		string sql;
        		
        		////////////XREF//////////////           		
        		
                	sql =
        		"SELECT xref_id " +
        		"FROM SubProgram WHERE id=" + functionID;
        		dbcmd.CommandText = sql;
        		reader = dbcmd.ExecuteReader();
        		reader.Read();
                	functionXrefID=reader.GetValue(0).ToString();                	
		        		        
		        ///////////////////////////////////////////
		        
		        ////////////SOURCE REGION//////////////           		
        		
                	sql =
        		"SELECT offset, length, file_id " +
        		"FROM SourceRegion WHERE xref_id=" + functionXrefID;
        		dbcmd.CommandText = sql;
        		reader = dbcmd.ExecuteReader();
        		reader.Read();
                	sourceInitLine=Convert.ToInt16(reader.GetValue(0));
                	sourceEndLine=sourceInitLine+Convert.ToInt16(reader.GetValue(1))-1; 
                	//Console.WriteLine(sourceInitLine - sourceEndLine); 
                	fileSourceID=reader.GetValue(2).ToString();               	
		        		        
		        ///////////////////////////////////////////
		        
		        //Regions inside functions source-------------------
		        
		        //Transition Context: obtain transition_id (met_max y wcet?)
		        sql =
        		"SELECT transition_id, wcet " +
        		"FROM TransitionContext WHERE context_id=" + functionID;
        		dbcmd.CommandText = sql;
        		reader = dbcmd.ExecuteReader();
        		//Console.WriteLine();
        		int maxTransitionWcet=0;
        		string maxTransitionWcetID="1";
        		while(reader.Read()) {

                	        if(Convert.ToInt16(reader.GetValue(1))>maxTransitionWcet){
                	                maxTransitionWcetID=reader.GetValue(0).ToString();
                	                maxTransitionWcet=Convert.ToInt16(reader.GetValue(1));
                	        }
                	        
                	}
                	
                	//xref id
                	string maxTransitionWcetXrefID="";
                	sql =
        		"SELECT xref_id " +
        		"FROM Transition WHERE id=" + maxTransitionWcetID;
        		dbcmd.CommandText = sql;
        		reader = dbcmd.ExecuteReader();
        		//Console.WriteLine();
        		while(reader.Read()) {
                	        maxTransitionWcetXrefID=reader.GetValue(0).ToString();
                	}
                	
                	//source
                	
                	sql =
        		"SELECT offset, length " +
        		"FROM SourceRegion WHERE xref_id=" + maxTransitionWcetXrefID;
        		dbcmd.CommandText = sql;
        		reader = dbcmd.ExecuteReader();
        		reader.Read();
                	transitionSourceInitLine=Convert.ToInt16(reader.GetValue(0));
                	transitionSourceEndLine=transitionSourceInitLine+Convert.ToInt16(reader.GetValue(1))-1; 
		        
		        
		        //----------------------------------------------------
		        
        		
        		////////////FUNCTION SOURCE CODE FILE//////////////           		
        		
                	sql =
        		"SELECT contents " +
        		"FROM SourceFile WHERE id=" + fileSourceID;
        		dbcmd.CommandText = sql;
        		reader = dbcmd.ExecuteReader();
        		while(reader.Read()) {
                		byte[] buf = (byte[]) reader.GetValue(0);
                		System.Text.Encoding enc = System.Text.Encoding.ASCII;
                		string myString = enc.GetString(buf);
                		//Console.WriteLine(myString);        		
                		
                		StringReader strReader = new StringReader(myString);
                		functionSource="";
                		int line=1;
                		string aLine;
                                while(strReader.Peek( ) != -1)
                                {
                                        aLine = strReader.ReadLine();
                                        if(line>=sourceInitLine && line<=sourceEndLine){
                                                /*if(line>=transitionSourceInitLine && line<=transitionSourceEndLine){
                                                        functionSource+=aLine+"\n";
                                                        Console.WriteLine(aLine);
                                                }else{
                                                        functionSource+=aLine+"\n";
                                                }*/
                                                functionSource+=aLine+"\n";
                                                
                        
                                        }else if(line>sourceEndLine){
                                                break;
                                        }
                                        //Console.WriteLine(aLine);
                                        aLine = "";
                                        line++;
                                        //Console.WriteLine(line);
                                }
                                //Console.WriteLine(functionSource);
		        }	//fileSource	        
		        /////////////////////////////////////////// 
		        
		        // clean up
        		reader.Close();
        		reader = null;
        		dbcmd.Dispose();
        		dbcmd = null;
	        
	        }//end DBExtractSourceData
	        
	        public static void DBExtractTransitionsData(){
                
                        transitionsData.Clear();      
                        IDbCommand dbcmd = dbcon.CreateCommand();
        		IDataReader reader;
        		string sql;
        		sql =
        		"SELECT transition_id, wcet " +
        		"FROM TransitionContext WHERE context_id=" + functionID;
        		dbcmd.CommandText = sql;
        		reader = dbcmd.ExecuteReader();

        		while(reader.Read()) {

                	        string id=reader.GetValue(0).ToString();
                	        double wcet=Convert.ToDouble(reader.GetValue(1))/factor;
                	        //Console.WriteLine(id);
                	        
                	        ///////////////////
                	        IDataReader reader2;
                        	sql =
                		"SELECT xref_id " +
                		"FROM Transition WHERE id=" + id;
                		dbcmd.CommandText = sql;
                		reader2 = dbcmd.ExecuteReader();
                		//Console.WriteLine();
                		while(reader2.Read()) {
                        	        string xref =reader2.GetValue(0).ToString();
                        	        //Console.WriteLine(xref);
                        	        
                        	        
                        	        ///////////////////////////////
                        	        IDataReader reader3;
                        	        sql =
                        		"SELECT offset, length " +
                        		"FROM SourceRegion WHERE xref_id=" + xref;
                        		dbcmd.CommandText = sql;
                        		reader3 = dbcmd.ExecuteReader();
                        		int tempInitLine=-1;
                        		int tempEndLine=-1;
                        		while(reader3.Read()) {
                        		        if(tempInitLine==-1){
                        		                //tempInitLine=Convert.ToInt16(reader3.GetValue(0))-sourceInitLine+1;
                        		                //tempEndLine=tempInitLine+Convert.ToInt16(reader3.GetValue(1))-1;
                        		                tempInitLine=Convert.ToInt16(reader3.GetValue(0));
                        		                tempEndLine=tempInitLine+Convert.ToInt16(reader3.GetValue(1))-1;
                        		                
                        		        }else{
                        		                //tempEndLine=Convert.ToInt16(reader3.GetValue(0))-sourceInitLine+Convert.ToInt16(reader3.GetValue(1))-1;
                        		                tempEndLine=Convert.ToInt16(reader3.GetValue(0))+Convert.ToInt16(reader3.GetValue(1))-1;
                        		                
                        		        }
                                        	

                                	}
                                	
                                	reader3.Close();
        		                reader3 = null;
                                	////////////////////
                                	//FILL DATA!!!!
                                	//Console.WriteLine(tempInitLine + "-" + tempEndLine);
                                	/*
                                	public int initLine;
                			public int endLine;
                			public int xrefID;
                			public double wcet;
                			*/
                                	transitionData.initLine=tempInitLine;
                                	transitionData.endLine=tempEndLine;
                                	transitionData.wcet=wcet;
                                	transitionData.xrefID=Convert.ToInt16(xref);
                                	//transitionsData.Add(Convert.ToInt16(id),transitionData);
                                	transitionsData.Add(transitionData);
                                	
                                	
                                	
                        	}
                        	
                        	reader2.Close();
        		        reader2 = null;
                	        ///////////////////
                	        
                	        
                	}
                	
                	// clean up
        		reader.Close();
        		reader = null;
        		dbcmd.Dispose();
        		dbcmd = null;
        		        		
        	}//end DBExtractTransitionsData
	        
	        public static void Init(){	
	        
	                ids2Names.Clear();
	                //ids2Names.TrimToSize ();
	                validIDs.Clear();
	                validIDs.TrimToSize ();

                        DBOpen();
                        
        		DBExtractFreq();
        		
        		DBExtractVersion();
        	
        		DBExtractContext();
        		
        		DBExtractValidIDs();
        		
        		DBExtractFunctionData();
        		
        		DBExtractEndToEndData();
        		
        		DBExtractSourceData();
        		
        		DBExtractTransitionsData();
        		
        		DBClose();
        		
        		
        	}//end Init
        	
                public static string NameToID(string name){
                       string id="1";	        	                
        	       /*for(int i=1; i<=ids2Names.Count;i++){
        	                if(ids2Names[i.ToString()] == name){
        	                        id=i.ToString();
        	                        break;
        	                }
        	       }*/
        	       foreach (KeyValuePair<string, string> id2name in ids2Names){
                           
                           if(id2name.Value==name){
                                id=id2name.Key;
                                break;
                           }
                       }
                       
        	       return id;
                       
        		
        	}//end NameToID
	
	}//end DataFunctions class
	
	
}//end namespace
