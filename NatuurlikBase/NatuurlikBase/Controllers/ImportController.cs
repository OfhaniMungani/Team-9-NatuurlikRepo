
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NatuurlikBase.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace natuurlik.Controllers;
[Authorize(Roles = SR.Role_Admin)]

public class ImportController : Controller
{
    private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;
    private IConfiguration Configuration;
    public ImportController(Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment, IConfiguration _configuration)
    {
        Environment = _environment;
        Configuration = _configuration;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(IFormFile postedFile)
    {
        if (postedFile != null)
        {
            string Ext = Path.GetExtension(postedFile.FileName);
            if (Ext == ".xls" || Ext == ".xlsx")
            {
                //Create a Folder.
                string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Save the uploaded Excel file.
                string fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                //Read the connection string for the Excel file.
                string conString = this.Configuration.GetConnectionString("ExcelConString");
                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                //Insert the Data read from the Excel file to Database Table.
                conString = @"Data Source=.;Initial Catalog=NatuurlikDBTest;Trusted_Connection=True;";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        sqlBulkCopy.DestinationTableName = "dbo.PaymentReminder";

                        //[OPTIONAL]: Map the Excel columns with that of the database table.
                        sqlBulkCopy.ColumnMappings.Add("Id", "Id");
                        sqlBulkCopy.ColumnMappings.Add("Days", "Days");
                        sqlBulkCopy.ColumnMappings.Add("Value", "Value");
                        sqlBulkCopy.ColumnMappings.Add("Active", "Active");
                        try
                        {
                            con.Open();
                            sqlBulkCopy.WriteToServer(dt);
                            TempData["upload"] = "Reseller Payment Reminder Time Imported Successfully";
                        }
                        catch (Exception exception)
                        {
                            // loop through all inner exceptions to see if any relate to a constraint failure

                            TempData["x"] = "Excel file contains null values or wrong column name(s), Please check your file";

                        }
                        finally
                        {
                            if (con != null && con.State == ConnectionState.Open)
                            {
                                con.Close();

                            }
                            // con.Close();


                        }

                    }
                }
            }
            else { TempData["x"] = "Please upload the Excel file in correct format"; }

        }
        else { TempData["x"] = "Please choose an Excel file to upload"; }
        return RedirectToAction("Index", "PaymentReminder");
    }
    [HttpPost]
    public IActionResult country(IFormFile postedFile)
    {
        if (postedFile != null)
        {
            string Ext = Path.GetExtension(postedFile.FileName);
            if (Ext == ".xls" || Ext == ".xlsx")
            {
                //Create a Folder.
                string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Save the uploaded Excel file.
                string fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                //Read the connection string for the Excel file.
                string conString = this.Configuration.GetConnectionString("ExcelConString");
                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                //Insert the Data read from the Excel file to Database Table.
                conString = @"Data Source=.;Initial Catalog=NatuurlikDBTest;Trusted_Connection=True;";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name.
                        sqlBulkCopy.DestinationTableName = "dbo.Country";
                        // sqlBulkCopy.DestinationTableName = "dbo.Province";
                        //[OPTIONAL]: Map the Excel columns with that of the database table.
                        sqlBulkCopy.ColumnMappings.Add("Id", "Id");
                        sqlBulkCopy.ColumnMappings.Add("CountryName", "CountryName");

                        // sqlBulkCopy.ColumnMappings.Add("ProvinceName", "ProvinceName");
                        // sqlBulkCopy.ColumnMappings.Add("CountryId", "CountryId");
                        try
                        {
                            con.Open();
                            sqlBulkCopy.WriteToServer(dt);
                            TempData["upload"] = "Country Imported Successfully";
                        }
                        catch (Exception exception)
                        {
                            // loop through all inner exceptions to see if any relate to a constraint failure

                            TempData["x"] = "Excel file contains null values or wrong column name(s), Please check your file";

                        }
                        finally
                        {
                            if (con != null && con.State == ConnectionState.Open)
                            {
                                con.Close();

                            }
                            // con.Close();


                        }

                    }
                }
            }
            else { TempData["x"] = "Please upload the Excel file in correct format"; }

        }
        else { TempData["x"] = "Please choose an Excel file to upload"; }
        return RedirectToAction("Create", "Countries");
    }
    [HttpPost]
    public IActionResult province(IFormFile postedFile)
    {
        if (postedFile != null)
        {
            string Ext = Path.GetExtension(postedFile.FileName);
            if (Ext == ".xls" || Ext == ".xlsx")
            {
                //Create a Folder.
                string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Save the uploaded Excel file.
                string fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                //Read the connection string for the Excel file.
                string conString = this.Configuration.GetConnectionString("ExcelConString");
                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                //Insert the Data read from the Excel file to Database Table.
                conString = @"Data Source=.;Initial Catalog=NatuurlikDBTest;Trusted_Connection=True;";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name.
                        sqlBulkCopy.DestinationTableName = "dbo.Province";
                        // sqlBulkCopy.DestinationTableName = "dbo.Province";
                        //[OPTIONAL]: Map the Excel columns with that of the database table.
                        sqlBulkCopy.ColumnMappings.Add("Id", "Id");
                        sqlBulkCopy.ColumnMappings.Add("ProvinceName", "ProvinceName");
                         sqlBulkCopy.ColumnMappings.Add("CountryId", "CountryId");
                        try
                        {
                            con.Open();
                            sqlBulkCopy.WriteToServer(dt);
                            TempData["upload"] = "Province(s) Imported Successfully";
                        }
                        catch (Exception exception)
                        {
                            // loop through all inner exceptions to see if any relate to a constraint failure

                            TempData["x"] = "Excel file contains null values or wrong column name(s), Please check your file";

                        }
                        finally
                        {
                            if (con != null && con.State == ConnectionState.Open)
                            {
                                con.Close();

                            }
                            // con.Close();


                        }

                    }
                }
            }
            else { TempData["x"] = "Please upload the Excel file in correct format"; }

        }
        else { TempData["x"] = "Please choose an Excel file to upload"; }
        return RedirectToAction("Create","Province");
    }
    [HttpPost]
    public IActionResult city(IFormFile postedFile)
    {
        if (postedFile != null)
        {
            string Ext = Path.GetExtension(postedFile.FileName);
            if (Ext == ".xls" || Ext == ".xlsx")
            {
                //Create a Folder.
                string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Save the uploaded Excel file.
                string fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                //Read the connection string for the Excel file.
                string conString = this.Configuration.GetConnectionString("ExcelConString");
                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                //Insert the Data read from the Excel file to Database Table.
                conString = @"Data Source=.;Initial Catalog=NatuurlikDBTest;Trusted_Connection=True;";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name.
                        sqlBulkCopy.DestinationTableName = "dbo.City";
                     
                        //[OPTIONAL]: Map the Excel columns with that of the database table.
                        sqlBulkCopy.ColumnMappings.Add("Id", "Id");
                        sqlBulkCopy.ColumnMappings.Add("CityName", "CityName");

                         sqlBulkCopy.ColumnMappings.Add("ProvinceId", "ProvinceId");
                     
                        try
                        {
                            con.Open();
                            sqlBulkCopy.WriteToServer(dt);
                            TempData["upload"] = "Cities Imported Successfully";
                        }
                        catch (Exception exception)
                        {
                            // loop through all inner exceptions to see if any relate to a constraint failure

                            TempData["x"] = "Excel file contains null values or wrong column name(s), Please check your file";

                        }
                        finally
                        {
                            if (con != null && con.State == ConnectionState.Open)
                            {
                                con.Close();

                            }
                            // con.Close();


                        }

                    }
                }
            }
            else { TempData["x"] = "Please upload the Excel file in correct format"; }

        }
        else { TempData["x"] = "Please choose an Excel file to upload"; }
        return RedirectToAction("Create", "Cities");
    }
    [HttpPost]
    public IActionResult suburb(IFormFile postedFile)
    {
        if (postedFile != null)
        {
            string Ext = Path.GetExtension(postedFile.FileName);
            if (Ext == ".xls" || Ext == ".xlsx")
            {
                //Create a Folder.
                string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Save the uploaded Excel file.
                string fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                //Read the connection string for the Excel file.
                string conString = this.Configuration.GetConnectionString("ExcelConString");
                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                //Insert the Data read from the Excel file to Database Table.
                conString = @"Data Source=.;Initial Catalog=NatuurlikDBTest;Trusted_Connection=True;";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name.
                        sqlBulkCopy.DestinationTableName = "dbo.Suburb";
                        // sqlBulkCopy.DestinationTableName = "dbo.Province";
                        //[OPTIONAL]: Map the Excel columns with that of the database table.
                        sqlBulkCopy.ColumnMappings.Add("Id", "Id");
                        sqlBulkCopy.ColumnMappings.Add("SuburbName", "SuburbName");

                        sqlBulkCopy.ColumnMappings.Add("PostalCode", "PostalCode");
                        sqlBulkCopy.ColumnMappings.Add("CityId", "CityId");
                        try
                        {
                            con.Open();
                            sqlBulkCopy.WriteToServer(dt);
                            TempData["upload"] = "Suburb(s) Imported Successfully";
                        }
                        catch (Exception exception)
                        {
                            // loop through all inner exceptions to see if any relate to a constraint failure

                            TempData["x"] = "Excel file contains null values or wrong column name(s), Please check your file";

                        }
                        finally
                        {
                            if (con != null && con.State == ConnectionState.Open)
                            {
                                con.Close();

                            }
                            // con.Close();


                        }

                    }
                }
            }
            else { TempData["x"] = "Please upload the Excel file in correct format"; }

        }
        else { TempData["x"] = "Please choose an Excel file to upload"; }
        return RedirectToAction("Create","Suburbs") ;
    }
}