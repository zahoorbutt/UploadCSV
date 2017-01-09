using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EntityFramework.BulkInsert.Extensions;
using System.Web;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Configuration;

namespace WaweUploadFileChallenge.Models
{
    public class UploadFile
    {       //zahoorbutt

        public static DataTable ProcessUpload(string fileLocation)
        {
            WaweDBEntities context = new WaweDBEntities();            
            
            bool result = false;
            DataTable dtFile = new DataTable();                              
            dtFile = ProcessCSV(fileLocation);
            if (dtFile != null)
            {
                result = InsertFileDataInDatabase(dtFile, context);
            }
          
             return dtFile;
        }
        /// <summary>
        /// Process the file supplied and process the CSV to a dynamic datatable
        /// </summary>
        /// <param name="fileLocation">String</param>
        /// <returns>DataTable</returns>
        private static DataTable ProcessCSV(string fileLocation)
        {
            //Set up our variables 
            string Feedback = string.Empty;
            string line = string.Empty;
            string[] strArray ;
            DataTable dt = new DataTable();
            DataRow row;

            // work out where we should split on comma, but not in a sentance
            Regex rex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            //Get the file data into stream reader
            StreamReader sr = new StreamReader(fileLocation);
          
            //Read the first line and split the string at , with our regular express in to an array
            line = sr.ReadLine();
            strArray = rex.Split(line);

            // Add columns
           
                dt.Columns.Add(strArray[0], typeof(DateTime));
                dt.Columns.Add(strArray[1], typeof(string));
                dt.Columns.Add(strArray[2], typeof(string));
                dt.Columns.Add(strArray[3], typeof(string));
                dt.Columns.Add(strArray[4], typeof(string));
                dt.Columns.Add(strArray[5], typeof(decimal));
                dt.Columns.Add(strArray[6], typeof(string));
                dt.Columns.Add(strArray[7], typeof(decimal));
                
            //Read each line in the CVS file and add in the row

            while (!sr.EndOfStream )
            {
                line = sr.ReadLine();
                row = dt.NewRow();
              //  dr = t.NewRow();      
                string[] datarow = rex.Split(line);
                //remove double quotes from address column
                for (int i = 0; i < datarow.Count(); i++)
                {
                    datarow[i] = datarow[i].Replace("\"", "");
                }

                //  add current value to data row
                row.ItemArray = datarow;
                dt.Rows.Add(row); 
            }
            
            //Tidy Streameader up
            sr.Dispose();
                                   
            //return a the new DataTable
            return dt;
        }
        private static bool InsertFileDataInDatabase(DataTable tblFileData, WaweDBEntities context)
        {
            bool result = true;

            try
            {
                foreach (DataRow row in tblFileData.Rows)
                {
                    context.tblUploads.Add(new tblUpload
                    {
                        Date = Convert.ToDateTime(row[0]),
                        Category = row[1].ToString(),
                        EmployeeName = row[2].ToString(),
                        EmployeeAddress = row[3].ToString(),
                        ExpenseDescription = row[4].ToString(),
                        PreTaxAmount = Convert.ToDecimal(row[5]),
                        TaxName = row[6].ToString(),
                        TaxAmount = Convert.ToDecimal(row[7])
                    });
                    
                } context.SaveChanges();
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception (ex.Message);
            }
             result = true;
            return result;
        }
     
    }
}