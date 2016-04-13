using FriendsWithPaws.Common;
using FriendsWithPaws.Data;
using Windows.ApplicationModel.Search;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.Storage;

// The Grid App template is documented at http://go.microsoft.com/fwlink/?LinkId=234226

namespace FriendsWithPaws
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        void OnSuggestionsRequested(SearchPane sender, SearchPaneSuggestionsRequestedEventArgs args)
        {
            string query = args.QueryText.ToLower();
            string[] terms = { "beagle", "yorkshire terrier", "basset hound", "australian shepherd" };
            List<string> termList = new List<string>() { "beagle", "yorkshire terrier", "basset hound", "australian shepherd" };

            foreach (var term in termList)
            {
                if (term.StartsWith(query)) args.Request.SearchSuggestionCollection.AppendQuerySuggestion(term);
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active

            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                if (!string.IsNullOrEmpty(args.Arguments)) ((Frame)Window.Current.Content).Navigate(typeof(ItemDetailPage), args.Arguments);
                Window.Current.Activate();
                return;
            }
            
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }

                if (args.PreviousExecutionState == ApplicationExecutionState.Running)
                {
                    Window.Current.Activate();
                    return;
                }
                await BreedDataSource.LoadDataAsync();

                SearchPane.GetForCurrentView().SuggestionsRequested += OnSuggestionsRequested;
                // SearchPane.GetForCurrentView().SearchHistoryEnabled = false;
               // SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;

                if (!string.IsNullOrEmpty(args.Arguments))
                {
                    rootFrame.Navigate(typeof(ItemDetailPage), args.Arguments);
                    Window.Current.Content = rootFrame;
                    Window.Current.Activate();
                    return;
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }
            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(GroupedItemsPage), "AllGroups"))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

   

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        /// <summary>
        /// Invoked when the application is activated to display search results.
        /// </summary>
        /// <param name="args">Details about the activation request.</param>
        protected async override void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.NotRunning || args.PreviousExecutionState == ApplicationExecutionState.ClosedByUser
                || args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                await BreedDataSource.LoadDataAsync();
                SearchPane.GetForCurrentView().SuggestionsRequested += OnSuggestionsRequested;
            }
            // TODO: Register the Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().QuerySubmitted
            // event in OnWindowCreated to speed up searches once the application is already running

            // If the Window isn't already using Frame navigation, insert our own Frame
            var previousContent = Window.Current.Content;
            var frame = previousContent as Frame;

            // If the app does not contain a top-level frame, it is possible that this 
            // is the initial launch of the app. Typically this method and OnLaunched 
            // in App.xaml.cs can call a common method.
            if (frame == null)
            {
                // Create a Frame to act as the navigation context and associate it with
                // a SuspensionManager key
                frame = new Frame();
                FriendsWithPaws.Common.SuspensionManager.RegisterFrame(frame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await FriendsWithPaws.Common.SuspensionManager.RestoreAsync();
                    }
                    catch (FriendsWithPaws.Common.SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }
            }

            frame.Navigate(typeof(SearchResultsPage), args.QueryText);
            Window.Current.Content = frame;

            // Ensure the current window is active
            Window.Current.Activate();
        }

        public void applicationsettings()
        {
            ApplicationDataContainer localsettings = ApplicationData.Current.LocalSettings;
            localsettings.Values["simplesettings"] = "Simple settings example";
        }
       
    }
}
