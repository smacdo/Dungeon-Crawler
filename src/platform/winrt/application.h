
ref class DungeonCrawlerApplication : public IFrameworkView
{
public:
    DungeonCrawlerApplication();

    virtual void Initialize( CoreApplicationView^ applicationView );
    virtual void SetWindow( CoreWindow^ window );
    virtual void Load( String^ entryPoint );
    virtual void Run();
    virtual void Uninitialize();
};
