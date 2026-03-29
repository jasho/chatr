# Chat Implementation Plan

## Scope
Implement end-to-end basic chat: SignalR hub on the server, MAUI app connects via HubConnection, user types/sends messages, messages appear on all instances.

## Architecture

```
ChatR.Common
  └── ChatMessage (record: Sender, Text, SentAt)
  └── ChatHubConstants (HubPath, SendMessage, ReceiveMessage)

ChatR.Server.App
  └── Hubs/ChatHub.cs   — SendMessage(sender, text) broadcasts to Clients.All
  └── Program.cs        — app.MapHub<ChatHub>("/chathub")

ChatR.Maui.App
  └── Models/AppSettings.cs        — { ServerUrl }
  └── Services/IChatService.cs     — interface
  └── Services/ChatService.cs      — HubConnection wrapper; fires MessageReceived event
  └── ViewModels/ChatPageViewModel — messages collection, send command, connect/disconnect
  └── Pages/ChatPage.xaml          — CollectionView + Entry + Button
  └── Configuration/appsettings.Development.json  — ServerUrl = dev tunnel URL
```

## Key Design Decisions
- `ChatMessage` is in `ChatR.Common` — shared reference, no duplication
- `ChatService` registered as **singleton** — connection persists for app lifetime
- Username = `DeviceInfo.Current.Name` — distinguishes instances on different devices
- `ViewModelBase` gets an `OnDisappearingAsync` virtual method; `ContentPageBase` calls it from `OnDisappearing`
- `ChatPageViewModel.LoadDataAsync` = connects SignalR; `OnDisappearingAsync` = disconnects

## Dev Tunnel Setup (manual, one-time for local Android testing)
1. `winget install Microsoft.DevTunnel`
2. `devtunnel user login`
3. `devtunnel host -p <server-port> --allow-anonymous`
4. Copy printed `https://<id>.devtunnels.ms` URL → `appsettings.Development.json` as `AppSettings.ServerUrl`

Note: `appsettings.Development.json` is git-ignored.

## XAML Chat UI (sketch)
```xml
<Grid RowDefinitions="*,Auto">
    <CollectionView Grid.Row="0" ItemsSource="{Binding Messages}">
        <CollectionView.ItemTemplate>
            <DataTemplate x:DataType="ChatMessage">
                <VerticalStackLayout Padding="8,4">
                    <Label Text="{Binding Sender}" FontAttributes="Bold" FontSize="12" />
                    <Label Text="{Binding Text}" />
                </VerticalStackLayout>
            </DataTemplate>
        </CollectionView.ItemTemplate>
    </CollectionView>
    <Grid Grid.Row="1" ColumnDefinitions="*,Auto" Padding="8">
        <Entry Grid.Column="0" Placeholder="Type a message..." Text="{Binding MessageText}" />
        <Button Grid.Column="1" Text="Send" Command="{Binding SendMessageCommand}" />
    </Grid>
</Grid>
```

## Todos

1. **common-models** — ChatMessage record + ChatHubConstants in ChatR.Common
2. **server-hub** — ChatHub + Program.cs mapping + Common project reference
3. **maui-signalr-pkg** — Add SignalR.Client NuGet + ChatR.Common project ref to MAUI app
4. **maui-global-imports** — Add ChatR.Common to GlobalUsings.cs + Imports.cs
5. **maui-appsettings-model** — Models/AppSettings.cs + IOptions registration
6. **maui-chat-service** — IChatService + ChatService + DI registration
7. **maui-viewmodel** — ChatPageViewModel fully implemented; OnDisappearingAsync added to base
8. **maui-chat-ui** — ChatPage.xaml with messages list + compose row
9. **appshell-chat-first** — Chat moved to first ShellContent
10. **dev-tunnel-docs** — appsettings.Development.json updated with placeholder + comment
11. **verify-build** — dotnet build ChatR.slnx → 0 errors
