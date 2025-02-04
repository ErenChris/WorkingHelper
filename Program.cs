﻿using NPOI;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.Util;
using System.IO;
using System;
using WorkingHelper.Handler;
using HtmlAgilityPack;
using WorkingHelper.Models;
using System.Collections.Generic;
using WorkingHelper.ExcelHandler;
using System.Configuration;

namespace WorkingHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            //获取当前工作路径
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            //被操作Excel文档路径名
            string appConfig_filePath = ConfigurationManager.AppSettings["ExcelPath"];
            string FileName = dir + appConfig_filePath;

            //获取Summary界面数据
            ExcelDataFromSummaryHTMLModel excelDataModel_get;
            IFillingExcelDataModelFromSummaryHTML fillingExcelDataModelFromHTML = new FillingExcelDataModelFromSummaryHTML();
            excelDataModel_get = fillingExcelDataModelFromHTML.StartCheckStation();
            //读取重测数据，生成Excel时使用

            //初始化数据容器List
            List<RetestUnitModel> GCRetestUnits = new List<RetestUnitModel>();
            List<RetestUnitModel> FFRetestUnits = new List<RetestUnitModel>();
            List<RetestUnitModel> GTRetestUnits = new List<RetestUnitModel>();
            List<RetestUnitModel> GT2RetestUnits = new List<RetestUnitModel>();
            //获取重测机台数据存储于数据容器中

            if (int.Parse(excelDataModel_get.RetestSheet_GC_RetestCount) != 0)
            {
                GCRetestUnits = HTMLToModelOfRetest.GetRetestUnitList(GCRetestUnits, "GC");
            }
            if (int.Parse(excelDataModel_get.RetestSheet_FF_RetestCount) != 0)
            {
                FFRetestUnits = HTMLToModelOfRetest.GetRetestUnitList(FFRetestUnits, "FF");
            }
            if (int.Parse(excelDataModel_get.RetestSheet_GT_RetestCount) != 0)
            {
                GTRetestUnits = HTMLToModelOfRetest.GetRetestUnitList(GTRetestUnits, "GT");
            }
            if (int.Parse(excelDataModel_get.RetestSheet_GT2_RetestCount) != 0)
            {
                GT2RetestUnits = HTMLToModelOfRetest.GetRetestUnitList(GT2RetestUnits, "GT2");
            }

            RowCounter rowCounter = new RowCounter(GCRetestUnits, FFRetestUnits, GTRetestUnits, GT2RetestUnits, excelDataModel_get);
            //string Path = @"D:\Desktop\111.xlsx";
            //HtmlDocument HtmlDocumentContainer = new HtmlDocument();
            //HtmlNode node;

            //string xPath = "//*[@id=\"analysis-drop-container\"]/div/div[3]/div/div[2]/div/table/tbody/tr[1]/td[1]";

            //Operate Excel
            //XSSFWorkbook dailyReportWorkBook = new XSSFWorkbook();
            //ISheet sheet = dailyReportWorkBook.CreateSheet("FirstSheet");
            //IRow row = sheet.CreateRow(5);
            //ICell cell = row.CreateCell(3);
            //cell.SetCellValue("测试2");

            //初始化ExcelOpreator对象
            YieldSheetExcelOpreator yieldSheetExcelOpreator = new YieldSheetExcelOpreator(FileName);

            //excelOpreator.ReviseExcelValue(ExcelOpreator.SheetEnum.yieldSheet, 10, 3, "ErenChris");
            int rowsNum = yieldSheetExcelOpreator.GetLastRowIndex(YieldSheetExcelOpreator.SheetEnum.yieldSheet);
            yieldSheetExcelOpreator.YieldSheetFilling(rowCounter, excelDataModel_get, GCRetestUnits, FFRetestUnits, GTRetestUnits, GT2RetestUnits);

            RetestSheetExcelOperator retestSheetExcelOperator = new RetestSheetExcelOperator(FileName);
            retestSheetExcelOperator.RetestSheetFilling(rowCounter, excelDataModel_get, GCRetestUnits, FFRetestUnits, GTRetestUnits, GT2RetestUnits);
            //using (FileStream FS = new FileStream(Path, FileMode.Create, FileAccess.Write))
            //{
            //    dailyReportWorkBook.Write(FS);
            //}

            Console.WriteLine("Done!");

            Console.ReadLine();
        }
    }
}
