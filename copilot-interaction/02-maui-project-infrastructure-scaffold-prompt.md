For the next part - create a plan to scaffold infrastructure code for the MAUI application.
Take inspiration from this repository: https://github.com/jasho/cookbook-maui
MVVM architecture. ViewModelBase class, ContentPageBase class, I want the mechanism for loading data in VM when page is appearing.
All pages should inherit from ContentPageBase and inject ViewModel as constructor dependency.
I want to use CommunityToolkit.Maui and its ObservableObject implementation of INotifyPropertyChanged.
Support for loading appsettings.json and optional appsettings.development.json should be implemented.
RoutingService should be included to enable navigation and all current routes should be registered in it.