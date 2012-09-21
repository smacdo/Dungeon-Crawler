#include "platform/winrt/application.h"

DungeonCrawlerApplication::DungeonCrawlerApplication()
{
}

void DungeonCrawlerApplication::Run()
{
    // Make sure our game object is alive
    assert( mpGame != NULL && "Game core must be initialized" );

    auto dispatcher = CoreWindow::GetForCurrentThread()->Dispatcher;

    while ( mWindowClosed )
    {
        dispatcher->ProcessEvents( CoreProcessEventsOption::ProcessAllIfPresent );
        mpGame->update();
        mpGame->render();
    }
}

void DungeonCrawlerApplication::SetWindow( CoreWindow^ window )
{
    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^,WindowSizeChangedEventArgs^>(
                this,
                &DungeonCrawlerApplication::OnWindowSizeChanged
    );
}

void DungeonCrawlerApplication::OnWindowSizeChanged(
        _In_ CoreWindow^ sender,
        _In_ WindowSizeChangedEventArgs^ args )
{
    if ( mWindow->Bounds.Width  != mWindowBounds.Width ||
         mWindow->Bounds.Height != mWindowBounds.Height )
    {
        mpGame->resize( mWindowBounds.Width, mWindowBounds.Height );
    }
}

void DungeonCrawlerApplication::OnWindowActivationChanged(
        CoreWindow^ sender,
        WindowActivatedEventArgs^ args )
{

}
