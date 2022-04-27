using System.Runtime.InteropServices;

namespace StaxBar;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // TODO: ==> Se mettre à jour quand un fenêtre est ouverte ou fermée
        // TODO: Corriger la taille des icones
        // TODO: Rendre les items réordonnables par drag-and-drop
        // TODO: Bouton "Start"
        // TODO: Heure
        // TODO: Tray
        // TODO: Minimiser si on re-clique sur le même item
        // TODO: Right-click, menu contextuel
        // TODO: Shift-click, open new instance


        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        ProcessesPaneUI ui = new ProcessesPaneUI();
        EventWatcherAsync eventWatcher = new EventWatcherAsync(() => ui.RefreshData());

        //new ProcessesWatcher2().Init("notepad");

        //Span<ProcessWatcher3> watcher3 = stackalloc ProcessWatcher3[1];
        //watcher3[0].Init("notepad");

        //Application.Run(new Form1());
        Application.Run(ui);

    }
}
