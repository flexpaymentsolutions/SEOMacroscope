﻿/*

  This file is part of SEOMacroscope.

  Copyright 2017 Jason Holland.

  The GitHub repository may be found at:

    https://github.com/nazuke/SEOMacroscope

  Foobar is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  Foobar is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Windows.Forms;

namespace SEOMacroscope
{

  public partial class MacroscopeMainForm : Form, IMacroscopeTaskController
  {

    /** Report Save Dialogue Boxes ********************************************/

    private const int CsvReportMegabytesRamRequired = 64;

    /**************************************************************************/

    private void CallbackSaveOverviewCsvReport ( object sender, EventArgs e )
    {
      
      SaveFileDialog Dialog = new SaveFileDialog ();
      Dialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
      Dialog.FilterIndex = 2;
      Dialog.RestoreDirectory = true;
      Dialog.DefaultExt = "xlsx";
      Dialog.AddExtension = true;
      Dialog.FileName = "Macroscope-Overview.xlsx";

      if( Dialog.ShowDialog() == DialogResult.OK )
      {

        string Path = Dialog.FileName;
        MacroscopeExcelOverviewReport msExcelReport = new MacroscopeExcelOverviewReport ();

        try
        {
          if( Macroscope.MemoryGuard( RequiredMegabytes: CsvReportMegabytesRamRequired ) )
          {
            msExcelReport.WriteXslx( this.JobMaster, Path );
          }
        }
        catch( MacroscopeInsufficientMemoryException ex )
        {
          this.DialogueBoxError( "Error saving Overview Excel Report", ex.Message );       
        }
        catch( MacroscopeSaveExcelFileException ex )
        {
          this.DialogueBoxError( "Error saving Overview Excel Report", ex.Message );
        }
        catch( Exception ex )
        {
          this.DialogueBoxError( "Error saving Overview Excel Report", ex.Message );
        }
        
      }

      Dialog.Dispose();
                
    }

    /** -------------------------------------------------------------------- **/  

    private void CallbackSaveErrorsCsvReport ( object sender, EventArgs e )
    {

      SaveFileDialog Dialog = new SaveFileDialog ();
      Dialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
      Dialog.FilterIndex = 2;
      Dialog.RestoreDirectory = true;
      Dialog.DefaultExt = "xlsx";
      Dialog.AddExtension = true;
      Dialog.FileName = "Macroscope-Errors.xlsx";

      if( Dialog.ShowDialog() == DialogResult.OK )
      {

        string Path = Dialog.FileName;
        MacroscopeExcelErrorsReport msExcelReport = new MacroscopeExcelErrorsReport ();

        try
        {
          if( Macroscope.MemoryGuard( RequiredMegabytes: CsvReportMegabytesRamRequired ) )
          {
            msExcelReport.WriteXslx( this.JobMaster, Path );
          }
        }
        catch( MacroscopeInsufficientMemoryException ex )
        {
          this.DialogueBoxError( "Error saving Errors Excel Report", ex.Message );       
        }
        catch( MacroscopeSaveExcelFileException ex )
        {
          this.DialogueBoxError( "Error saving Errors Excel Report", ex.Message );
        }
        catch( Exception ex )
        {
          this.DialogueBoxError( "Error saving Errors Excel Report", ex.Message );
        }

      }

      Dialog.Dispose();

    }

    /** -------------------------------------------------------------------- **/

    private void CallbackSaveBrokenLinksCsvReport ( object sender, EventArgs e )
    {

      SaveFileDialog Dialog = new SaveFileDialog ();
      Dialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
      Dialog.FilterIndex = 2;
      Dialog.RestoreDirectory = true;
      Dialog.DefaultExt = "xlsx";
      Dialog.AddExtension = true;
      Dialog.FileName = "Macroscope-Broken-Links.xlsx";

      if( Dialog.ShowDialog() == DialogResult.OK )
      {

        string Path = Dialog.FileName;
        MacroscopeExcelBrokenLinksReport msExcelReport = new MacroscopeExcelBrokenLinksReport ();

        try
        {
          if( Macroscope.MemoryGuard( RequiredMegabytes: CsvReportMegabytesRamRequired ) )
          {
            msExcelReport.WriteXslx( this.JobMaster, Path );
          }
        }
        catch( MacroscopeInsufficientMemoryException ex )
        {
          this.DialogueBoxError( "Error saving Broken Links Excel Report", ex.Message );       
        }
        catch( MacroscopeSaveExcelFileException ex )
        {
          this.DialogueBoxError( "Error saving Broken Links Excel Report", ex.Message );
        }
        catch( Exception ex )
        {
          this.DialogueBoxError( "Error saving Broken Links Excel Report", ex.Message );
        }

      }

      Dialog.Dispose();

    }

    /** -------------------------------------------------------------------- **/

    private void CallbackSaveLanguagesCsvReport ( object sender, EventArgs e )
    {

      SaveFileDialog Dialog = new SaveFileDialog ();
      Dialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
      Dialog.FilterIndex = 2;
      Dialog.RestoreDirectory = true;
      Dialog.DefaultExt = "xlsx";
      Dialog.AddExtension = true;
      Dialog.FileName = "Macroscope-Languages.xlsx";

      if( Dialog.ShowDialog() == DialogResult.OK )
      {

        string Path = Dialog.FileName;
        MacroscopeExcelLanguagesReport msExcelReport = new MacroscopeExcelLanguagesReport ();

        try
        {
          if( Macroscope.MemoryGuard( RequiredMegabytes: CsvReportMegabytesRamRequired ) )
          {
            Cursor.Current = Cursors.WaitCursor;
            msExcelReport.WriteXslx( this.JobMaster, Path );
            Cursor.Current = Cursors.Default;
          }
        }
        catch( MacroscopeInsufficientMemoryException ex )
        {
          this.DialogueBoxError( "Error saving HrefLang Excel Report", ex.Message );       
        }
        catch( MacroscopeSaveExcelFileException ex )
        {
          this.DialogueBoxError( "Error saving HrefLang Excel Report", ex.Message );
        }
        catch( Exception ex )
        {
          this.DialogueBoxError( "Error saving HrefLang Excel Report", ex.Message );
        }
        finally
        {
          Cursor.Current = Cursors.Default;
        }

      }

      Dialog.Dispose();

    }

    /** -------------------------------------------------------------------- **/

    private void CallbackSavePageMetadataCsvReportTitles ( object sender, EventArgs e )
    {
      this.CallbackSavePageMetadataCsvReport(
        sender: sender,
        e: e,
        SelectedOutputWorksheet: MacroscopeCsvPageMetadataReport.OutputWorksheet.TITLES,
        OutputFilename: "Macroscope-Page-Metadata-Titles.csv"
      );
    }

    private void CallbackSavePageMetadataCsvReportDescriptions ( object sender, EventArgs e )
    {
      this.CallbackSavePageMetadataCsvReport(
        sender: sender,
        e: e,
        SelectedOutputWorksheet: MacroscopeCsvPageMetadataReport.OutputWorksheet.DESCRIPTIONS,
        OutputFilename: "Macroscope-Page-Metadata-Descriptions.csv"
      );
    }

    private void CallbackSavePageMetadataCsvReportKeywords ( object sender, EventArgs e )
    {
      this.CallbackSavePageMetadataCsvReport(
        sender: sender,
        e: e,
        SelectedOutputWorksheet: MacroscopeCsvPageMetadataReport.OutputWorksheet.KEYWORDS,
        OutputFilename: "Macroscope-Page-Metadata-Keywords.csv"
      );
    }

    private void CallbackSavePageMetadataCsvReport (
      object sender,
      EventArgs e,
      MacroscopeCsvPageMetadataReport.OutputWorksheet SelectedOutputWorksheet,
      string OutputFilename
    )
    {

      SaveFileDialog Dialog = new SaveFileDialog ();
      Dialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
      Dialog.FilterIndex = 2;
      Dialog.RestoreDirectory = true;
      Dialog.DefaultExt = "csv";
      Dialog.AddExtension = true;
      Dialog.FileName = OutputFilename;

      if( Dialog.ShowDialog() == DialogResult.OK )
      {

        string Path = Dialog.FileName;
        MacroscopeCsvPageMetadataReport msCsvReport = new MacroscopeCsvPageMetadataReport ();

        try
        {
          
          if( Macroscope.MemoryGuard( RequiredMegabytes: CsvReportMegabytesRamRequired ) )
          {
            
            Cursor.Current = Cursors.WaitCursor;
            
            msCsvReport.WriteCsv(
              JobMaster: this.JobMaster,
              SelectedOutputWorksheet: SelectedOutputWorksheet,
              OutputFilename: Path
            );
            
            Cursor.Current = Cursors.Default;
          
          }
          
        }
        catch( MacroscopeInsufficientMemoryException ex )
        {
          this.DialogueBoxError( "Error saving Page Metadata CSV Report", ex.Message );       
        }
        catch( MacroscopeSaveExcelFileException ex )
        {
          this.DialogueBoxError( "Error saving Page Metadata CSV Report", ex.Message );
        }
        catch( Exception ex )
        {
          this.DialogueBoxError( "Error saving Page Metadata CSV Report", ex.Message );
        }
        finally
        {
          Cursor.Current = Cursors.Default;
        }

      }

      Dialog.Dispose();

    }

    /** -------------------------------------------------------------------- **/

    private void CallbackSavePageContentsCsvReport ( object sender, EventArgs e )
    {

      SaveFileDialog Dialog = new SaveFileDialog ();
      Dialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
      Dialog.FilterIndex = 2;
      Dialog.RestoreDirectory = true;
      Dialog.DefaultExt = "xlsx";
      Dialog.AddExtension = true;
      Dialog.FileName = "Macroscope-Page-Contents.xlsx";

      if( Dialog.ShowDialog() == DialogResult.OK )
      {

        string Path = Dialog.FileName;
        MacroscopeExcelPageContentsReport msExcelReport = new MacroscopeExcelPageContentsReport ();

        try
        {
          if( Macroscope.MemoryGuard( RequiredMegabytes: CsvReportMegabytesRamRequired ) )
          {
            Cursor.Current = Cursors.WaitCursor;
            msExcelReport.WriteXslx( this.JobMaster, Path );
            Cursor.Current = Cursors.Default;
          }
        }
        catch( MacroscopeInsufficientMemoryException ex )
        {
          this.DialogueBoxError( "Error saving Page Contents Excel Report", ex.Message );       
        }
        catch( MacroscopeSaveExcelFileException ex )
        {
          this.DialogueBoxError( "Error saving Page Contents Excel Report", ex.Message );
        }
        catch( Exception ex )
        {
          this.DialogueBoxError( "Error saving Page Contents Excel Report", ex.Message );
        }
        finally
        {
          Cursor.Current = Cursors.Default;
        }

      }

      Dialog.Dispose();

    }

    /** -------------------------------------------------------------------- **/

    private void CallbackSaveUriAnalysisCsvReport ( object sender, EventArgs e )
    {

      SaveFileDialog Dialog = new SaveFileDialog ();
      Dialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
      Dialog.FilterIndex = 2;
      Dialog.RestoreDirectory = true;
      Dialog.DefaultExt = "xlsx";
      Dialog.AddExtension = true;
      Dialog.FileName = "Macroscope-URI-Analysis.xlsx";

      if( Dialog.ShowDialog() == DialogResult.OK )
      {

        string Path = Dialog.FileName;
        MacroscopeExcelUriReport msExcelReport = new MacroscopeExcelUriReport ();

        try
        {
          if( Macroscope.MemoryGuard( RequiredMegabytes: CsvReportMegabytesRamRequired ) )
          {
            Cursor.Current = Cursors.WaitCursor;
            msExcelReport.WriteXslx( this.JobMaster, Path );
            Cursor.Current = Cursors.Default;
          }
        }
        catch( MacroscopeInsufficientMemoryException ex )
        {
          this.DialogueBoxError( "Error saving URI Analysis Excel Report", ex.Message );       
        }
        catch( MacroscopeSaveExcelFileException ex )
        {
          this.DialogueBoxError( "Error saving URI Analysis Excel Report", ex.Message );
        }
        catch( Exception ex )
        {
          this.DialogueBoxError( "Error saving URI Analysis Excel Report", ex.Message );
        }
        finally
        {
          Cursor.Current = Cursors.Default;
        }

      }

      Dialog.Dispose();

    }

    /** -------------------------------------------------------------------- **/

    private void CallbackSaveRedirectsCsvReport ( object sender, EventArgs e )
    {

      SaveFileDialog Dialog = new SaveFileDialog ();
      
      Dialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
      Dialog.FilterIndex = 2;
      Dialog.RestoreDirectory = true;
      Dialog.DefaultExt = "xlsx";
      Dialog.AddExtension = true;
      Dialog.FileName = "Macroscope-Redirects.xlsx";

      if( Dialog.ShowDialog() == DialogResult.OK )
      {

        string Path = Dialog.FileName;
        MacroscopeExcelRedirectsReport msExcelReport = new MacroscopeExcelRedirectsReport ();

        try
        {
          if( Macroscope.MemoryGuard( RequiredMegabytes: CsvReportMegabytesRamRequired ) )
          {
            Cursor.Current = Cursors.WaitCursor;
            msExcelReport.WriteXslx( this.JobMaster, Path );
            Cursor.Current = Cursors.Default;
          }
        }
        catch( MacroscopeInsufficientMemoryException ex )
        {
          this.DialogueBoxError( "Error saving Redirects Excel Report", ex.Message );       
        }
        catch( MacroscopeSaveExcelFileException ex )
        {
          this.DialogueBoxError( "Error saving Redirects Excel Report", ex.Message );
        }
        catch( Exception ex )
        {
          this.DialogueBoxError( "Error saving Redirects Excel Report", ex.Message );
        }
        finally
        {
          Cursor.Current = Cursors.Default;
        }

      }

      Dialog.Dispose();

    }

    /** -------------------------------------------------------------------- **/

    private void CallbackSaveKeywordAnalysisCsvReport ( object sender, EventArgs e )
    {

      SaveFileDialog Dialog = new SaveFileDialog ();

      Dialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
      Dialog.FilterIndex = 2;
      Dialog.RestoreDirectory = true;
      Dialog.DefaultExt = "xlsx";
      Dialog.AddExtension = true;
      Dialog.FileName = "Macroscope-Keyword-Analysis.xlsx";
      
      if( Dialog.ShowDialog() == DialogResult.OK )
      {

        string Path = Dialog.FileName;

        MacroscopeTriplePercentageProgressForm ProgressForm = new MacroscopeTriplePercentageProgressForm ( MainForm: this );

        MacroscopeExcelKeywordAnalysisReport msExcelReport = new MacroscopeExcelKeywordAnalysisReport (
                                                               ProgressFormDialogue: ProgressForm
                                                             );
        
        try
        {
          if( Macroscope.MemoryGuard( RequiredMegabytes: CsvReportMegabytesRamRequired ) )
          {
            Cursor.Current = Cursors.WaitCursor;
            msExcelReport.WriteXslx( this.JobMaster, Path );
            Cursor.Current = Cursors.Default;
          }
        }
        catch( MacroscopeInsufficientMemoryException ex )
        {
          this.DialogueBoxError( "Error saving Keyword Analysis Excel Report", ex.Message );       
        }
        catch( MacroscopeSaveExcelFileException ex )
        {
          this.DialogueBoxError( "Error saving Keyword Analysis Excel Report", ex.Message );
        }
        catch( Exception ex )
        {
          this.DialogueBoxError( "Error saving Keyword Analysis Excel Report", ex.Message );
        }
        finally
        {
          Cursor.Current = Cursors.Default;
        }
    
      }
      
      Dialog.Dispose();
    
    }

    /** -------------------------------------------------------------------- **/

    private void CallbackSaveDuplicateContentCsvReport ( object sender, EventArgs e )
    {

      SaveFileDialog Dialog = new SaveFileDialog ();
      
      Dialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
      Dialog.FilterIndex = 2;
      Dialog.RestoreDirectory = true;
      Dialog.DefaultExt = "xlsx";
      Dialog.AddExtension = true;
      Dialog.FileName = "Macroscope-Duplicate-Content.xlsx";
      
      if( Dialog.ShowDialog() == DialogResult.OK )
      {

        string Path = Dialog.FileName;

        MacroscopeTriplePercentageProgressForm ProgressForm = new MacroscopeTriplePercentageProgressForm ( MainForm: this );

        MacroscopeExcelDuplicateContent msExcelReport = new MacroscopeExcelDuplicateContent (
                                                          ProgressFormDialogue: ProgressForm
                                                        );
        try
        {
          if( Macroscope.MemoryGuard( RequiredMegabytes: CsvReportMegabytesRamRequired ) )
          {
            Cursor.Current = Cursors.WaitCursor;
            msExcelReport.WriteXslx( JobMaster: this.JobMaster, OutputFilename: Path );
            Cursor.Current = Cursors.Default;
          }
        }
        catch( MacroscopeInsufficientMemoryException ex )
        {
          this.DialogueBoxError( "Error saving Duplicate Content Excel Report", ex.Message );       
        }
        catch( MacroscopeSaveExcelFileException ex )
        {
          this.DialogueBoxError( "Error saving Duplicate Content Excel Report", ex.Message );
        }
        catch( Exception ex )
        {
          this.DialogueBoxError( "Error saving Duplicate Content Excel Report", ex.Message );
        }
        finally
        {
          ProgressForm.DoClose();
          Cursor.Current = Cursors.Default;
        }
      
      }
      
      Dialog.Dispose();
    
    }

    /** -------------------------------------------------------------------- **/

    private void CallbackSaveContactDetailsCsvReport ( object sender, EventArgs e )
    {
    
      SaveFileDialog Dialog = new SaveFileDialog ();
      
      Dialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
      Dialog.FilterIndex = 2;
      Dialog.RestoreDirectory = true;
      Dialog.DefaultExt = "xlsx";
      Dialog.AddExtension = true;
      Dialog.FileName = "Macroscope-Contact-Details.xlsx";

      if( Dialog.ShowDialog() == DialogResult.OK )
      {

        string Path = Dialog.FileName;
        MacroscopeExcelContactDetailsReport msExcelReport = new MacroscopeExcelContactDetailsReport ();

        try
        {
          if( Macroscope.MemoryGuard( RequiredMegabytes: CsvReportMegabytesRamRequired ) )
          {
            Cursor.Current = Cursors.WaitCursor;
            msExcelReport.WriteXslx( this.JobMaster, Path );
            Cursor.Current = Cursors.Default;
          }
        }
        catch( MacroscopeInsufficientMemoryException ex )
        {
          this.DialogueBoxError( "Error saving Contact Details Excel Report", ex.Message );       
        }
        catch( MacroscopeSaveExcelFileException ex )
        {
          this.DialogueBoxError( "Error saving Contact Details Excel Report", ex.Message );
        }
        catch( Exception ex )
        {
          this.DialogueBoxError( "Error saving Contact Details Excel Report", ex.Message );
        }
        finally
        {
          Cursor.Current = Cursors.Default;
        }

      }

      Dialog.Dispose();

    }

    /** -------------------------------------------------------------------- **/

    private void CallbackSaveRemarksCsvReport ( object sender, EventArgs e )
    {

      SaveFileDialog Dialog = new SaveFileDialog ();
      
      Dialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
      Dialog.FilterIndex = 2;
      Dialog.RestoreDirectory = true;
      Dialog.DefaultExt = "xlsx";
      Dialog.AddExtension = true;
      Dialog.FileName = "Macroscope-Remarks.xlsx";

      if( Dialog.ShowDialog() == DialogResult.OK )
      {

        string Path = Dialog.FileName;
        MaroscopeExcelRemarksReport msExcelReport = new MaroscopeExcelRemarksReport ();

        try
        {
          if( Macroscope.MemoryGuard( RequiredMegabytes: CsvReportMegabytesRamRequired ) )
          {
            Cursor.Current = Cursors.WaitCursor;
            msExcelReport.WriteXslx( this.JobMaster, Path );
            Cursor.Current = Cursors.Default;
          }
        }
        catch( MacroscopeInsufficientMemoryException ex )
        {
          this.DialogueBoxError( "Error saving Remarks Excel Report", ex.Message );       
        }
        catch( MacroscopeSaveExcelFileException ex )
        {
          this.DialogueBoxError( "Error saving Remarks Excel Report", ex.Message );
        }
        catch( Exception ex )
        {
          this.DialogueBoxError( "Error saving Remarks Excel Report", ex.Message );
        }
        finally
        {
          Cursor.Current = Cursors.Default;
        }

      }

      Dialog.Dispose();

    }

    /** -------------------------------------------------------------------- **/

    private void CallbackSaveCustomFilterCsvReport ( object sender, EventArgs e )
    {

      SaveFileDialog Dialog = new SaveFileDialog ();
      
      Dialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
      Dialog.FilterIndex = 2;
      Dialog.RestoreDirectory = true;
      Dialog.DefaultExt = "xlsx";
      Dialog.AddExtension = true;
      Dialog.FileName = "Macroscope-Custom-Filters.xlsx";

      if( Dialog.ShowDialog() == DialogResult.OK )
      {

        string Path = Dialog.FileName;
        MacroscopeExcelCustomFilterReport msExcelReport = new MacroscopeExcelCustomFilterReport ( NewCustomFilter: this.CustomFilter );

        try
        {
          if( Macroscope.MemoryGuard( RequiredMegabytes: CsvReportMegabytesRamRequired ) )
          {
            Cursor.Current = Cursors.WaitCursor;
            msExcelReport.WriteXslx( this.JobMaster, Path );
            Cursor.Current = Cursors.Default;
          }
        }
        catch( MacroscopeInsufficientMemoryException ex )
        {
          this.DialogueBoxError( "Error saving Custom Filters Excel Report", ex.Message );       
        }
        catch( MacroscopeSaveExcelFileException ex )
        {
          this.DialogueBoxError( "Error saving Custom Filters Excel Report", ex.Message );
        }
        catch( Exception ex )
        {
          this.DialogueBoxError( "Error saving Custom Filters Excel Report", ex.Message );
        }
        finally
        {
          Cursor.Current = Cursors.Default;
        }

      }

      Dialog.Dispose();

    }

    /** -------------------------------------------------------------------- **/

    private void CallbackSaveDataExtractorsCsvReport ( object sender, EventArgs e )
    {

      SaveFileDialog Dialog = new SaveFileDialog ();
      
      Dialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
      Dialog.FilterIndex = 2;
      Dialog.RestoreDirectory = true;
      Dialog.DefaultExt = "xlsx";
      Dialog.AddExtension = true;
      Dialog.FileName = "Macroscope-Data-Extractors.xlsx";

      if( Dialog.ShowDialog() == DialogResult.OK )
      {

        string Path = Dialog.FileName;
        MacroscopeExcelDataExtractorReport msExcelReport;
        
        msExcelReport = new MacroscopeExcelDataExtractorReport (
          NewDataExtractorCssSelectors: this.DataExtractorCssSelectors,
          NewDataExtractorRegexes: this.DataExtractorRegexes,
          NewDataExtractorXpaths: this.DataExtractorXpaths
        );

        try
        {
          if( Macroscope.MemoryGuard( RequiredMegabytes: CsvReportMegabytesRamRequired ) )
          {
            Cursor.Current = Cursors.WaitCursor;
            msExcelReport.WriteXslx( this.JobMaster, Path );
            Cursor.Current = Cursors.Default;
          }
        }
        catch( MacroscopeInsufficientMemoryException ex )
        {
          this.DialogueBoxError( "Error saving Data Extractors Excel Report", ex.Message );       
        }
        catch( MacroscopeSaveExcelFileException ex )
        {
          this.DialogueBoxError( "Error saving Data Extractors Excel Report", ex.Message );
        }
        catch( Exception ex )
        {
          this.DialogueBoxError( "Error saving Data Extractors Excel Report", ex.Message );
        }
        finally
        {
          Cursor.Current = Cursors.Default;
        }

      }

      Dialog.Dispose();

    }

    /**************************************************************************/

  }

}