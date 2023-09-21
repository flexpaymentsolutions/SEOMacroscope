﻿/*

  This file is part of SEOMacroscope.

  Copyright 2020 Jason Holland.

  The GitHub repository may be found at:

    https://github.com/nazuke/SEOMacroscope

  SEOMacroscope is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  SEOMacroscope is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with SEOMacroscope.  If not, see <http://www.gnu.org/licenses/>.

 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SEOMacroscope
{

  /// <summary>
  /// Description of MacroscopeJobMaster.
  /// </summary>

  [Serializable()]
  public class MacroscopeJobMaster : Macroscope, IDisposable
  {

    /**************************************************************************/

    //private EventLog JobMasterLog;
    //private Guid JobGuid;
    //private long JobMasterInstanceCounter = 1;

    private MacroscopeConstants.RunTimeMode RunTimeMode;

    [field: NonSerialized()]
    private IMacroscopeTaskController TaskController;
    private MacroscopeCredentialsHttp CredentialsHttp;

    [field: NonSerialized()]
    private MacroscopeHttpTwoClient Client;

    private MacroscopeDocumentCollection DocCollection;
    private MacroscopeAllowedHosts AllowedHosts;

    private MacroscopeNamedQueue<MacroscopeJobItem> NamedQueueJobItems;
    private MacroscopeNamedQueue<string> NamedQueue;

    private CookieContainer CookieJar;

    private MacroscopeRobots Robots;
    private MacroscopeIncludeExcludeUrls IncludeExcludeUrls;

    private MacroscopeCustomFilters CustomFilter;

    private MacroscopeDataExtractorCssSelectors DataExtractorCssSelectors;
    private MacroscopeDataExtractorRegexes DataExtractorRegexes;
    private MacroscopeDataExtractorXpaths DataExtractorXpaths;

    private int CrawlDelay;
    private int ThreadsMax;
    private bool ThreadsStop;
    private object ThreadsLock = new object();

    private bool WorkersStopped = true;

    private string StartUrl;

    private Object PagesFoundLock;
    private int PagesFound;

    private string ParentStartingDirectory;
    private string ChildStartingDirectory;

    private MacroscopeJobHistory JobHistory;

    private Dictionary<string, Dictionary<string, bool>> Progress;

    private Dictionary<string, string> Locales;

    private Dictionary<string, bool> BlockedByRobots;

    /**************************************************************************/

    public MacroscopeJobMaster ():this( MacroscopeConstants.RunTimeMode.LIVE )
    {
      // NO-OP
    }
    public MacroscopeJobMaster ( MacroscopeConstants.RunTimeMode JobRunTimeMode )
    {

      this.SuppressDebugMsg = false;

      this.TaskController = null;

      this.InitializeJobMaster( JobRunTimeMode: JobRunTimeMode );

    }

    /** -------------------------------------------------------------------- **/

    public MacroscopeJobMaster (
      MacroscopeConstants.RunTimeMode JobRunTimeMode,
      IMacroscopeTaskController TaskController
    )
    {

      this.SuppressDebugMsg = true;

      this.TaskController = TaskController;

      this.InitializeJobMaster( JobRunTimeMode: JobRunTimeMode );

    }

    /** Self Destruct Sequence ************************************************/

    public void Dispose ()
    {
      Dispose( true );
    }

    protected virtual void Dispose ( bool disposing )
    {
      try
      {
        if( this.DocCollection != null )
        {
          this.DocCollection.Dispose();
        }
      }
      catch( Exception )
      {
        // NO-OP
      }
    }

    /**************************************************************************/

    private void InitializeJobMaster ( MacroscopeConstants.RunTimeMode JobRunTimeMode )
    {

      /*
      {
        this.JobMasterLog = new EventLog ();
        this.JobMasterLog.Source = MacroscopeConstants.MainEventLogSourceName;
        this.JobGuid = Guid.NewGuid();
        this.LogEntry( string.Format( "Starting Job" ) );
      }
      */

      this.RunTimeMode = JobRunTimeMode;

      this.Client = new MacroscopeHttpTwoClient();

      if( this.TaskController != null )
      {
        this.CredentialsHttp = this.TaskController.IGetCredentialsHttp();
      }

      this.DocCollection = new MacroscopeDocumentCollection( JobMaster: this );
      this.AllowedHosts = new MacroscopeAllowedHosts();

      /** BEGIN: Named Queues *************************************************/

      this.NamedQueueJobItems = new MacroscopeNamedQueue<MacroscopeJobItem>();

      this.NamedQueueJobItems.CreateNamedQueue(
        Name: MacroscopeConstants.NamedQueueUrlList,
        QueueMode: MacroscopeNamedQueue<MacroscopeJobItem>.MODE.USE_HISTORY
      );

      this.NamedQueue = new MacroscopeNamedQueue<string>();

      {

        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayQueue );

        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayStructure );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayStructureLinkCounts );

        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayHierarchy );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayCanonicalAnalysis );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayHrefLang );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayErrors );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayHostnames );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayRedirectsAudit );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayRedirectChains );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayLinks );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayHyperlinks );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayUriAnalysis );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayOrphanedPages );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayPageTitles );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayPageDescriptions );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayPageKeywords );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayPageHeadings );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayPageText );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayStylesheets );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayImages );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayJavascripts );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayAudios );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayVideos );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplaySitemaps );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplaySitemapErrors );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplaySitemapsAudit );

        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayEmailAddresses );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayTelephoneNumbers );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayCustomFilters );

        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayDataExtractorsCssSelectors );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayDataExtractorsRegexes );
        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayDataExtractorsXpaths );

        this.NamedQueue.CreateNamedQueue( Name: MacroscopeConstants.NamedQueueDisplayRemarks );

      }

      /** END: Named Queues ***************************************************/

      this.CrawlDelay = 0;

      this.AdjustThreadsMax();
      this.ThreadsStop = false;

      this.PagesFoundLock = new Object();
      this.SetPagesFound( Value: 0 );

      {
        this.ParentStartingDirectory = "";
        this.ChildStartingDirectory = "";
      }

      this.JobHistory = new MacroscopeJobHistory();

      this.InitProgress();

      this.Locales = new Dictionary<string, string>( 32 );

      this.CookieJar = new CookieContainer();

      this.Robots = new MacroscopeRobots();
      this.BlockedByRobots = new Dictionary<string, bool>();

    }

    /** Deserialization *******************************************************/

    public void InitializeAfterDeserialization ( IMacroscopeTaskController NewTaskController )
    {

      this.TaskController = NewTaskController;
      this.Client = new MacroscopeHttpTwoClient();

      this.Robots.InitializeAfterDeserialization();

      return;

    }

    /** Event Log *************************************************************/

    /*
    private void LogEntry ( string Message )
    {

      string [] Messages = {
        string.Format(
          "{0} :: {1}",
          this.JobGuid,
          Message
        )
      };

      EventInstance ev = new EventInstance ( JobMasterInstanceCounter, 1 );

      this.JobMasterLog.WriteEvent( ev, Messages );

      this.JobMasterInstanceCounter++;

    }
    */

    /** Runtime Mode **********************************************************/

    public void SetRunTimeMode ( MacroscopeConstants.RunTimeMode JobRunTimeMode )
    {
      this.RunTimeMode = JobRunTimeMode;
    }

    public MacroscopeConstants.RunTimeMode GetRunTimeMode ()
    {
      return ( this.RunTimeMode );
    }

    /** HTTP Client ***********************************************************/

    public MacroscopeHttpTwoClient GetHttpClient ()
    {
      return ( this.Client );
    }

    /** Credentials **********************************************************/

    public MacroscopeCredentialsHttp GetCredentialsHttp ()
    {
      return ( this.CredentialsHttp );
    }

    /** Include/Exclude URLs **************************************************/

    public void SetIncludeExcludeUrls ( MacroscopeIncludeExcludeUrls NewIncludeExcludeUrls )
    {
      this.IncludeExcludeUrls = NewIncludeExcludeUrls;
    }

    public MacroscopeIncludeExcludeUrls GetIncludeExcludeUrls ()
    {
      return ( this.IncludeExcludeUrls );
    }

    /** Custom Filter *********************************************************/

    public void SetCustomFilter ( MacroscopeCustomFilters NewCustomFilter )
    {
      this.CustomFilter = NewCustomFilter;
    }

    public MacroscopeCustomFilters GetCustomFilter ()
    {
      return ( this.CustomFilter );
    }

    /** Data Extractors *********************************************************/

    public void SetDataExtractorCssSelectors ( MacroscopeDataExtractorCssSelectors NewDataExtractor )
    {
      this.DataExtractorCssSelectors = NewDataExtractor;
    }

    public MacroscopeDataExtractorCssSelectors GetDataExtractorCssSelectors ()
    {
      return ( this.DataExtractorCssSelectors );
    }

    /** -------------------------------------------------------------------- **/

    public void SetDataExtractorRegexes ( MacroscopeDataExtractorRegexes NewDataExtractor )
    {
      this.DataExtractorRegexes = NewDataExtractor;
    }

    public MacroscopeDataExtractorRegexes GetDataExtractorRegexes ()
    {
      return ( this.DataExtractorRegexes );
    }

    /** -------------------------------------------------------------------- **/

    public void SetDataExtractorXpaths ( MacroscopeDataExtractorXpaths NewDataExtractor )
    {
      this.DataExtractorXpaths = NewDataExtractor;
    }

    public MacroscopeDataExtractorXpaths GetDataExtractorXpaths ()
    {
      return ( this.DataExtractorXpaths );
    }

    /** Execute Job ***********************************************************/

    public bool Execute ()
    {

      this.DebugMsg( string.Format( "Start URL: {0}", this.GetStartUrl() ) );

      MacroscopeHttpTwoClient.ClearCookieMonster();

      this.SetStartUrl( Url: MacroscopeHttpUrlUtils.SanitizeUrl( Url: this.GetStartUrl() ) );

      this.DocCollection.SetStartUrl( Url: this.GetStartUrl() );

      this.SetThreadsStop( Stopped: false );

      this.AllowedHosts.AddFromUrl( Url: this.GetStartUrl() );

      if( !this.PeekUrlQueue() )
      {

        { // Add robots.txt URL to queue
          if( MacroscopePreferencesManager.GetFollowRobotsProtocol() )
          {
            string RobotsUrl = MacroscopeRobots.GenerateRobotUrl( Url: this.GetStartUrl() );
            if( !string.IsNullOrEmpty( RobotsUrl ) )
            {
              this.AddUrlQueueItem( Url: RobotsUrl );
            }
          }
        }

        { // Add sitemap.xml URLs to queue
          if( MacroscopePreferencesManager.GetFetchXml() && MacroscopePreferencesManager.GetFollowSitemapLinks() )
          {
            MacroscopeSitemapPaths SitemapPaths = new MacroscopeSitemapPaths();
            foreach( string SitemapPath in SitemapPaths.IterateSitemapPaths() )
            {
              string SitemapUrl = MacroscopeSitemapPaths.GenerateSitemapUrl( Url: this.GetStartUrl(), SitemapPath: SitemapPath );
              if( !string.IsNullOrEmpty( SitemapUrl ) )
              {
                this.AddUrlQueueItem( Url: SitemapUrl );
              }
            }
          }
        }

        { // Add humans.txt URL to queue
          if( MacroscopePreferencesManager.GetProbeHumansText() )
          {
            string HumansUrl = MacroscopeHumans.GenerateHumansUrl( Url: this.GetStartUrl() );
            if( !string.IsNullOrEmpty( HumansUrl ) )
            {
              this.AddUrlQueueItem( Url: HumansUrl );
            }
          }
        }

        this.IncludeExcludeUrls.AddExplicitIncludeUrl( Url: this.GetStartUrl() );

        this.AddUrlQueueItem( Url: this.GetStartUrl() );

        foreach( MacroscopeDocument msDoc in this.GetDocCollection().IterateDocuments() )
        {
          this.ProcessOutlinks( msDoc: msDoc );
        }

      }

      this.ProbeRobotsFile( Url: this.GetStartUrl() );

      this.SetCrawlDelay( Url: this.GetStartUrl() );

      this.SpawnWorkers();

      this.DebugMsg( string.Format( "Pages Found: {0}", this.GetPagesFound() ) );

      if( this.TaskController != null )
      {
        this.TaskController.ICallbackScanComplete();
      }

      this.AddUpdateDisplayQueue( Url: this.GetStartUrl() );

      return ( true );

    }

    /** Manage Workers ********************************************************/

    private void SpawnWorkers ()
    {

      bool DoRun = true;

      while( DoRun == true )
      {

        this.WorkersStopped = false;

        if( this.GetThreadsStop() )
        {

          this.DebugMsg( string.Format( "SpawnWorkers: {0}", "STOPPING" ) );

          DoRun = false;
          break;

        }
        else
        {

          List<Task> TaskList = new List<Task>( this.ThreadsMax );

          for( int i = 0 ; i < this.ThreadsMax ; i++ )
          {

            Task CrawlTask;
            MacroscopeJobWorker JobWorker = new MacroscopeJobWorker( JobMaster: this );

            try
            {
              CrawlTask = new Task( () => JobWorker.Execute(), TaskCreationOptions.AttachedToParent );
              TaskList.Add( CrawlTask );
              CrawlTask.Start();
            }
            catch( Exception ex )
            {
              this.DebugMsg( string.Format( "TASK ERROR: {0}", ex.Message ) );
            }

            Thread.Sleep( 2000 ); // Allow each thread to spin up

          }

          foreach( Task CrawlTask in TaskList )
          {
            CrawlTask.Wait();
          }

          if( !this.PeekUrlQueue() )
          {
            DoRun = false;
          }

        }

      }

      this.GetDocCollection().AddWorkerRecalculateDocCollectionQueue();

      this.DebugMsg( string.Format( "SpawnWorkers: STOPPED" ) );

    }

    /** -------------------------------------------------------------------- **/

    public void NotifyWorkersFetched ( string Url )
    {

      this.DebugMsg( string.Format( "NotifyWorkersFetched: {0}", Url ) );

      this.IncPagesFound();

      this.AddUpdateDisplayQueue( Url: Url );

      this.GetDocCollection().AddWorkerRecalculateDocCollectionQueue();

      this.UpdateProgress( Url: Url, State: true );

    }

    /** -------------------------------------------------------------------- **/

    public void NotifyWorkersDone ()
    {

      this.GetDocCollection().AddWorkerRecalculateDocCollectionQueue();

      this.GetDocCollection().RecalculateDocCollection();

      this.GetDocCollection().RecalculateDocCollectionFinal();

      this.WorkersStopped = true;

    }

    /** -------------------------------------------------------------------- **/

    public void StopWorkers ()
    {
      this.SetThreadsStop( true );
    }

    /** -------------------------------------------------------------------- **/

    public bool AreWorkersStopped ()
    {
      return ( this.WorkersStopped );
    }

    /** Track Thread Count ****************************************************/

    private void SetThreadsStop ( bool Stopped )
    {
      this.ThreadsStop = Stopped;
    }

    /** -------------------------------------------------------------------- **/

    public bool GetThreadsStop ()
    {
      return ( this.ThreadsStop );
    }

    /** -------------------------------------------------------------------- **/

    private void AdjustThreadsMax ()
    {
      ThreadsMax = MacroscopePreferencesManager.GetMaxThreads();
    }

    /** Queues ****************************************************************/

    public void ClearAllQueues ()
    {
      this.NamedQueue.ClearAllNamedQueues();
    }

    /** Display Queue *********************************************************/

    public bool PeekUpdateDisplayQueue ()
    {
      return ( NamedQueue.PeekNamedQueue( MacroscopeConstants.NamedQueueDisplayQueue ) );
    }

    /** -------------------------------------------------------------------- **/

    public void AddUpdateDisplayQueue ( string Url )
    {

      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayQueue, Url );

      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayStructure, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayStructureLinkCounts, Url );

      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayHierarchy, Url );

      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayCanonicalAnalysis, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayHrefLang, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayErrors, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayHostnames, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayRedirectsAudit, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayRedirectChains, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayLinks, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayHyperlinks, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayUriAnalysis, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayOrphanedPages, Url );

      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayPageTitles, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayPageDescriptions, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayPageKeywords, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayPageHeadings, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayPageText, Url );

      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayStylesheets, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayImages, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayJavascripts, Url );

      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayAudios, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayVideos, Url );

      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplaySitemaps, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplaySitemapErrors, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplaySitemapsAudit, Url );

      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayEmailAddresses, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayTelephoneNumbers, Url );

      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayCustomFilters, Url );

      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayDataExtractorsCssSelectors, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayDataExtractorsRegexes, Url );
      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayDataExtractorsXpaths, Url );

      NamedQueue.AddToNamedQueue( MacroscopeConstants.NamedQueueDisplayRemarks, Url );

    }

    /** -------------------------------------------------------------------- **/

    public List<string> DrainDisplayQueueAsList ( string NamedQueueName )
    {
      return ( this.NamedQueue.DrainNamedQueueItemsAsList( NamedQueueName ) );
    }

    /** URL Queue *************************************************************/

    public MacroscopeJobItem[] GetUrlQueueAsArray ()
    {

      MacroscopeJobItem[] ItemsArray = null;

      try
      {
        ItemsArray = this.NamedQueueJobItems.GetNamedQueueItemsAsArray(
          MacroscopeConstants.NamedQueueUrlList
        );
      }
      catch( Exception ex )
      {
        this.DebugMsg( string.Format( "GetUrlQueueAsArray: {0}", ex.Message ) );
      }

      return ( ItemsArray );

    }

    /** -------------------------------------------------------------------- **/

    // TODO: Implement RedirectedFromUrl handling here:
    public void AddUrlQueueItem ( string Url, string RedirectedFromUrl = null )
    {

      string NewUrl = Url;
      MacroscopeJobItem JobItem = null;

      if( !MacroscopePreferencesManager.GetCheckExternalLinks() )
      {
        if( this.AllowedHosts.IsExternalUrl( Url: Url ) )
        {
          return;
        }
      }

      if( MacroscopePreferencesManager.GetIgnoreQueries() )
      {
        NewUrl = MacroscopeHttpUrlUtils.StripQueryString( Url: NewUrl );
      }

      if( MacroscopePreferencesManager.GetIgnoreHashFragments() )
      {
        NewUrl = MacroscopeHttpUrlUtils.StripHashFragment( Url: NewUrl );
      }

      if( this.JobHistory.SeenHistoryItem( Url: NewUrl ) )
      {
        this.DebugMsg( string.Format( "Already seen: {0}", Url ) );
      }
      else
      {

        try
        {

          if( !string.IsNullOrEmpty( RedirectedFromUrl ) )
          {
            JobItem = new MacroscopeJobItem( Url: NewUrl, RedirectedFromUrl: RedirectedFromUrl );
          }
          else
          {
            JobItem = new MacroscopeJobItem( Url: NewUrl );
          }

          this.NamedQueueJobItems.AddToNamedQueue(
            Name: MacroscopeConstants.NamedQueueUrlList,
            Item: JobItem
          );

          this.AddToProgress( Url: NewUrl );

          this.AddProbedParentFolderUrlsToUrlQueue( Url: NewUrl );

        }
        catch( MacroscopeNamedQueueException ex )
        {
          this.DebugMsg( string.Format( "AddUrlQueueItem: {0}", ex.Message ) );
        }

      }

      return;

    }

    /** -------------------------------------------------------------------- **/

    public void AddUrlQueueItem ( string Url, bool Check, bool Force = false )
    {

      bool Proceed = true;

      if( Check && MacroscopePreferencesManager.GetCrawlStrictUrlCheck() )
      {

        if( this.IncludeExcludeUrls.MatchesExcludeUrlPattern( Url: Url ) )
        {
          Proceed = false;
        }

        if( !this.IncludeExcludeUrls.MatchesIncludeUrlPattern( Url: Url ) )
        {
          Proceed = false;
        }

      }

      if( Proceed )
      {

        this.AddUrlQueueItem( Url: Url );

        this.AddProbedParentFolderUrlsToUrlQueue( Url: Url );

      }

      return;

    }

    /** -------------------------------------------------------------------- **/

    public MacroscopeJobItem GetUrlQueueItem ()
    {

      MacroscopeJobItem JobItem = this.NamedQueueJobItems.GetNamedQueueItem( Name: MacroscopeConstants.NamedQueueUrlList );

      return ( JobItem );

    }

    /** -------------------------------------------------------------------- **/

    public void ForgetUrlQueueItem ( string Url )
    {

      MacroscopeJobItem JobItem;
      string NewUrl = Url;
      bool Forgotten = false;

      if( MacroscopePreferencesManager.GetIgnoreQueries() )
      {
        NewUrl = MacroscopeHttpUrlUtils.StripQueryString( Url: NewUrl );
      }

      if( MacroscopePreferencesManager.GetIgnoreHashFragments() )
      {
        NewUrl = MacroscopeHttpUrlUtils.StripHashFragment( Url: NewUrl );
      }

      JobItem = new MacroscopeJobItem( Url: NewUrl );

      Forgotten = this.NamedQueueJobItems.ForgetNamedQueueItem(
        Name: MacroscopeConstants.NamedQueueUrlList,
        Item: JobItem
      );

    }

    /** -------------------------------------------------------------------- **/

    public List<MacroscopeJobItem> DrainUrlQueueAsList ()
    {

      List<MacroscopeJobItem> JobItems;

      JobItems = this.NamedQueueJobItems.DrainNamedQueueItemsAsList(
        Name: MacroscopeConstants.NamedQueueUrlList,
        Limit: 5
      );

      return ( JobItems );

    }

    /** -------------------------------------------------------------------- **/

    /*
     * Check to see if queue has items waiting in it.
    */
    public bool PeekUrlQueue ()
    {
      bool Peek = this.NamedQueueJobItems.PeekNamedQueue( Name: MacroscopeConstants.NamedQueueUrlList );
      return ( Peek );
    }

    /** -------------------------------------------------------------------- **/

    /*
     * Check to see if queue has a specific item waiting in it.
    */
    public bool PeekUrlQueueItem ( string Url )
    {
      MacroscopeJobItem JobItem = new MacroscopeJobItem( Url: Url );
      return ( this.NamedQueueJobItems.PeekNamedQueueItem( Name: MacroscopeConstants.NamedQueueUrlList, Item: JobItem ) );
    }

    /** -------------------------------------------------------------------- **/

    public int CountUrlQueueItems ()
    {
      return ( this.NamedQueueJobItems.CountNamedQueueItems( Name: MacroscopeConstants.NamedQueueUrlList ) );
    }

    /** -------------------------------------------------------------------- **/

    private void AddProbedParentFolderUrlsToUrlQueue ( string Url )
    {

      if( MacroscopePreferencesManager.GetProbeParentFolderUrls() )
      {

        if( this.AllowedHosts.IsInternalUrl( Url: Url ) )
        {

          List<string> ParentFolderUrls = MacroscopeHttpUrlUtils.GetParentFolderUrls( Url: Url );

          foreach( string ParentFolderUrl in ParentFolderUrls )
          {
            MacroscopeJobItem ParentJobItem = new MacroscopeJobItem( Url: ParentFolderUrl );
            this.NamedQueueJobItems.AddToNamedQueue( Name: MacroscopeConstants.NamedQueueUrlList, Item: ParentJobItem );
            this.AddToProgress( Url: ParentFolderUrl );
          }

        }

      }

    }

    /** Retry Broken Links ****************************************************/

    public void RetryBrokenLinks ()
    {

      foreach( MacroscopeDocument msDoc in this.DocCollection.IterateDocuments() )
      {

        switch( msDoc.GetStatusCode() )
        {

          // Bogus Range

          case 0:
            this.ResetLink( msDoc: msDoc );
            break;

          // 200 Range

          case HttpStatusCode.OK:
            break;

          // 400 Range

          case HttpStatusCode.BadRequest:
            this.ResetLink( msDoc: msDoc );
            break;
          case HttpStatusCode.Unauthorized:
            this.ResetLink( msDoc: msDoc );
            break;
          case HttpStatusCode.Forbidden:
            this.ResetLink( msDoc: msDoc );
            break;
          case HttpStatusCode.NotFound:
            this.ResetLink( msDoc: msDoc );
            break;
          case HttpStatusCode.Gone:
            this.ResetLink( msDoc: msDoc );
            break;
          case HttpStatusCode.RequestTimeout:
            this.ResetLink( msDoc: msDoc );
            break;
          case HttpStatusCode.RequestUriTooLong:
            this.ResetLink( msDoc: msDoc );
            break;

          // 500 Range

          case HttpStatusCode.InternalServerError:
            this.ResetLink( msDoc: msDoc );
            break;
          case HttpStatusCode.NotImplemented:
            this.ResetLink( msDoc: msDoc );
            break;
          case HttpStatusCode.BadGateway:
            this.ResetLink( msDoc: msDoc );
            break;
          case HttpStatusCode.ServiceUnavailable:
            this.ResetLink( msDoc: msDoc );
            break;
          case HttpStatusCode.GatewayTimeout:
            this.ResetLink( msDoc: msDoc );
            break;

          // Default

          default:
            break;

        }

      }

    }

    /** -------------------------------------------------------------------- **/

    public void RetryTimedOutLinks ()
    {

      foreach( MacroscopeDocument msDoc in this.DocCollection.IterateDocuments() )
      {

        switch( msDoc.GetStatusCode() )
        {

          case HttpStatusCode.RequestTimeout:
            this.ResetLink( msDoc: msDoc );
            break;

          case HttpStatusCode.GatewayTimeout:
            this.ResetLink( msDoc: msDoc );
            break;

          default:
            break;

        }

      }

    }

    /** -------------------------------------------------------------------- **/

    public void RetryLink ( string Url )
    {
      MacroscopeDocument msDoc = this.DocCollection.GetDocumentByUrl( Url: Url );
      this.ResetLink( msDoc: msDoc );
      return;
    }

    /** -------------------------------------------------------------------- **/

    public void RetryLink ( MacroscopeDocument msDoc )
    {
      this.ResetLink( msDoc: msDoc );
      return;
    }

    /** -------------------------------------------------------------------- **/

    private void ResetLink ( MacroscopeDocument msDoc )
    {

      if( msDoc != null )
      {

        string Url = msDoc.GetUrl();

        msDoc.SetIsDirty();

        this.ForgetUrlQueueItem( Url: Url );

        this.JobHistory.ResetHistoryItem( Url: Url );

        this.AddUrlQueueItem( Url: Url );

      }
      else
      {
        this.DebugMsg( string.Format( "ResetLink ERROR" ) );
      }

      return;

    }

    /** Start URL *************************************************************/

    public string SetStartUrl ( string Url )
    {

      this.StartUrl = Url;
      this.DetermineStartingDirectory();

      MacroscopePreferencesManager.CrawlHistoryPush( Url: Url );

      return ( this.StartUrl );

    }

    /** -------------------------------------------------------------------- **/

    public string GetStartUrl ()
    {
      return ( this.StartUrl );
    }

    /** -------------------------------------------------------------------- **/

    public string GetStartUriHostAndPort ()
    {
      Uri StartUri = new Uri( this.GetStartUrl() );
      string StartUriHostAndPort = null;
      if( StartUri != null )
      {
        if( StartUri.Port == 80 )
        {
          StartUriHostAndPort = StartUri.Host;
        }
        else
        {
          StartUriHostAndPort = string.Format( "{0}:{1}", StartUri.Host, StartUri.Port );
        }
      }
      return ( StartUriHostAndPort );
    }

    /** Pages Found Counter ***************************************************/

    public void SetPagesFound ( int Value )
    {
      lock( this.PagesFoundLock )
      {
        this.PagesFound = Value;
      }
    }

    /** -------------------------------------------------------------------- **/

    public int GetPagesFound ()
    {
      return ( this.PagesFound );
    }

    /** -------------------------------------------------------------------- **/

    private void IncPagesFound ()
    {
      lock( this.PagesFoundLock )
      {
        this.PagesFound += 1;
      }
    }

    /** Crawl Parent / Child Directories **************************************/

    private void DetermineStartingDirectory ()
    {
      string StartingUrl = MacroscopeHttpUrlUtils.DetermineStartingDirectory( Url: this.GetStartUrl() );
      this.SetParentStartingDirectory( Url: StartingUrl );
      this.SetChildStartingDirectory( Url: StartingUrl );
    }

    /** -------------------------------------------------------------------- **/

    private void SetParentStartingDirectory ( string Url )
    {
      this.ParentStartingDirectory = Url;
    }

    /** -------------------------------------------------------------------- **/

    public string GetParentStartingDirectory ()
    {
      return ( this.ParentStartingDirectory );
    }

    /** -------------------------------------------------------------------- **/

    private void SetChildStartingDirectory ( string Url )
    {
      this.ChildStartingDirectory = Url;
    }

    /** -------------------------------------------------------------------- **/

    public string GetChildStartingDirectory ()
    {
      return ( this.ChildStartingDirectory );
    }

    /** History ***************************************************************/

    public MacroscopeJobHistory GetJobHistory ()
    {
      return ( this.JobHistory );
    }

    /** Progress **************************************************************/

    private void InitProgress ()
    {
      this.Progress = new Dictionary<string, Dictionary<string, bool>>();
      this.Progress.Add( "list", new Dictionary<string, bool>( 4096 ) );
      this.Progress.Add( "done", new Dictionary<string, bool>( 4096 ) );
      this.Progress.Add( "wait", new Dictionary<string, bool>( 4096 ) );
    }

    /** -------------------------------------------------------------------- **/

    public void AddToProgress ( string Url )
    {

      if( this.AllowedHosts.IsAllowedFromUrl( Url ) )
      {
        this.DebugMsg( string.Format( "AddToProgress: INTERNAL: {0}", Url ) );

        lock( this.Progress )
        {

          if( !this.Progress[ "list" ].ContainsKey( Url ) )
          {

            this.Progress[ "list" ].Add( Url, false );

            if( !this.Progress[ "wait" ].ContainsKey( Url ) )
            {
              this.Progress[ "wait" ].Add( Url, true );
            }

          }

        }

      }
      else
      {
        this.DebugMsg( string.Format( "AddToProgress: EXTERNAL: {0}", Url ) );
      }

    }

    /** -------------------------------------------------------------------- **/

    public void UpdateProgress ( string Url, bool State )
    {

      if( State && this.AllowedHosts.IsAllowedFromUrl( Url ) )
      {

        this.DebugMsg( string.Format( "UpdateProgress: INTERNAL: {0}", Url ) );

        lock( this.Progress )
        {

          if( this.Progress[ "list" ].ContainsKey( Url ) )
          {

            this.Progress[ "list" ][ Url ] = true;

            if( this.Progress[ "wait" ].ContainsKey( Url ) )
            {
              this.Progress[ "wait" ].Remove( Url );
            }

            if( this.Progress[ "done" ].ContainsKey( Url ) )
            {
              this.Progress[ "done" ][ Url ] = true;
            }
            else
            {
              this.Progress[ "done" ].Add( Url, true );
            }

          }
          else
          {

            this.DebugMsg( string.Format( "UpdateProgress: NOT IN LIST: {0}", Url ) );

          }

        }

      }
      else
      {
        this.DebugMsg( string.Format( "UpdateProgress: EXTERNAL: {0} :: {1}", State, Url ) );
      }

    }

    /** -------------------------------------------------------------------- **/

    public List<decimal> GetProgress ()
    {

      List<decimal> Counts = new List<decimal>( 3 );

      lock( this.Progress )
      {
        Counts.Add( this.Progress[ "list" ].Count );
        Counts.Add( this.Progress[ "done" ].Count );
        Counts.Add( this.Progress[ "wait" ].Count );
      }

      return ( Counts );

    }

    /** Document Collection ***************************************************/

    public MacroscopeDocumentCollection GetDocCollection ()
    {
      return ( this.DocCollection );
    }

    /** -------------------------------------------------------------------- **/

    public void SetDocCollection ( MacroscopeDocumentCollection NewDocCollection )
    {
      this.DocCollection = NewDocCollection;
    }

    /** Allowed Hosts *********************************************************/

    public MacroscopeAllowedHosts GetAllowedHosts ()
    {
      return ( this.AllowedHosts );
    }

    /** Locales ***************************************************************/

    public Dictionary<string, string> GetLocales ()
    {
      return ( this.Locales );
    }

    /** -------------------------------------------------------------------- **/

    public void AddLocales ( string Locale )
    {
      lock( this.Locales )
      {
        if( !this.Locales.ContainsKey( Locale ) )
        {
          this.Locales[ Locale ] = Locale;
        }
      }
    }

    /** Cookie Jar ************************************************************/

    public CookieContainer GetCookieJar ()
    {
      return ( this.CookieJar );
    }

    /** Robots ****************************************************************/

    public MacroscopeRobots GetRobots ()
    {
      return ( this.Robots );
    }

    /** -------------------------------------------------------------------- **/

    private async void SetCrawlDelay ( string Url )
    {
      try
      {
        this.CrawlDelay = await this.Robots.GetCrawlDelay( Url );
      }
      catch( Exception ex )
      {
        this.DebugMsg( string.Format( "SetCrawlDelay: {0}", ex.Message ) );
      }
      return;
    }

    /** -------------------------------------------------------------------- **/

    public int GetCrawlDelay ()
    {
      return ( this.CrawlDelay );
    }

    /** -------------------------------------------------------------------- **/

    public async void ProbeRobotsFile ( string Url )
    {
      if( MacroscopePreferencesManager.GetFollowSitemapLinks() )
      {
        List<string> SitemapList = await Robots.GetSitemapsAsList( Url );
        if( SitemapList.Count > 0 )
        {
          for( int i = 0 ; i < SitemapList.Count ; i++ )
          {
            this.AddUrlQueueItem( Url: SitemapList[ i ] );
          }
        }
      }
    }

    /** -------------------------------------------------------------------- **/

    public void AddToBlockedByRobots ( string Url )
    {
      lock( this.BlockedByRobots )
      {
        if( !this.BlockedByRobots.ContainsKey( Url ) )
        {
          this.BlockedByRobots[ Url ] = true;
        }
      }
    }

    /** -------------------------------------------------------------------- **/

    public void RemoveFromBlockedByRobots ( string Url )
    {

      lock( this.BlockedByRobots )
      {

        if( this.BlockedByRobots.ContainsKey( Url ) )
        {
          this.BlockedByRobots.Remove( Url );
        }

      }

    }

    /** -------------------------------------------------------------------- **/

    public Dictionary<string, bool> GetBlockedByRobotsList ()
    {

      Dictionary<string, bool> Copy = new Dictionary<string, bool>();

      lock( this.BlockedByRobots )
      {

        foreach( string Url in this.BlockedByRobots.Keys )
        {
          Copy.Add( Url, this.BlockedByRobots[ Url ] );
        }

      }

      return ( Copy );

    }

    /** Process OutLinks ******************************************************/

    public void ProcessOutlinks ( MacroscopeDocument msDoc )
    {

      if(
        ( this.GetRunTimeMode() == MacroscopeConstants.RunTimeMode.LISTFILE )
        || ( this.GetRunTimeMode() == MacroscopeConstants.RunTimeMode.LISTTEXT )
        || ( this.GetRunTimeMode() == MacroscopeConstants.RunTimeMode.SITEMAP ) )
      {

        if( !MacroscopePreferencesManager.GetScanSitesInList() )
        {
          return;
        }

      }

      foreach( MacroscopeLink Outlink in msDoc.IterateOutlinks() )
      {

        MacroscopeConstants.InOutLinkType LinkType = Outlink.GetLinkType();
        bool Proceed = true;

        if( !Outlink.GetDoFollow() )
        {
          Proceed = false;
        }

        if( Outlink.GetTargetUrl() == null )
        {
          Proceed = false;
        }

        if( this.JobHistory.SeenHistoryItem( Url: Outlink.GetTargetUrl() ) )
        {
          Proceed = false;
        }

        switch( LinkType )
        {
          case MacroscopeConstants.InOutLinkType.CANONICAL:
            if( !MacroscopePreferencesManager.GetFollowCanonicalLinks() )
            {
              Proceed = false;
            }
            break;
          case MacroscopeConstants.InOutLinkType.ALTERNATE:
            if( !MacroscopePreferencesManager.GetFollowAlternateLinks() )
            {
              Proceed = false;
            }
            break;
          case MacroscopeConstants.InOutLinkType.STYLESHEET:
            if( !MacroscopePreferencesManager.GetFetchStylesheets() )
            {
              Proceed = false;
            }
            break;
          case MacroscopeConstants.InOutLinkType.SCRIPT:
            if( !MacroscopePreferencesManager.GetFetchJavascripts() )
            {
              Proceed = false;
            }
            break;
          case MacroscopeConstants.InOutLinkType.IMAGE:
            if( !MacroscopePreferencesManager.GetFetchImages() )
            {
              Proceed = false;
            }
            break;
          case MacroscopeConstants.InOutLinkType.FAVICON:
            if( !MacroscopePreferencesManager.GetFetchImages() )
            {
              Proceed = false;
            }
            break;
          case MacroscopeConstants.InOutLinkType.AUDIO:
            if( !MacroscopePreferencesManager.GetFetchAudio() )
            {
              Proceed = false;
            }
            break;
          case MacroscopeConstants.InOutLinkType.VIDEO:
            if( !MacroscopePreferencesManager.GetFetchVideo() )
            {
              Proceed = false;
            }
            break;
          case MacroscopeConstants.InOutLinkType.EMBED:
            if( !MacroscopePreferencesManager.GetFetchBinaries() )
            {
              Proceed = false;
            }
            break;
          case MacroscopeConstants.InOutLinkType.OBJECT:
            if( !MacroscopePreferencesManager.GetFetchBinaries() )
            {
              Proceed = false;
            }
            break;
          case MacroscopeConstants.InOutLinkType.SITEMAPTEXT:
            if( !MacroscopePreferencesManager.GetFetchXml() )
            {
              Proceed = false;
            }
            break;
          case MacroscopeConstants.InOutLinkType.SITEMAPXML:
            if( !MacroscopePreferencesManager.GetFetchXml() )
            {
              Proceed = false;
            }
            break;
          default:
            this.DebugMsg( string.Format( "Unhandled MacroscopeConstants.InOutLinkType: {0}", LinkType ) );
            break;
        }

        if( !MacroscopePreferencesManager.GetCheckExternalLinks() )
        {
          if( this.DocCollection != null )
          {
            MacroscopeAllowedHosts AllowedHosts = this.DocCollection.GetAllowedHosts();
            if( AllowedHosts != null )
            {
              if( !AllowedHosts.IsAllowedFromUrl( Url: Outlink.GetTargetUrl() ) )
              {
                Proceed = false;
              }
            }
          }
        }

        if( Proceed )
        {
          this.AddUrlQueueItem( Url: Outlink.GetTargetUrl(), Check: true );
        }

      }

    }

    /**************************************************************************/

  }

}
