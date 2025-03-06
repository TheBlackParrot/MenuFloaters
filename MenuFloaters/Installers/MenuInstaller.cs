using MenuFloaters.UI;
using JetBrains.Annotations;
using MenuFloaters.Managers;
using Zenject;

namespace MenuFloaters.Installers;

[UsedImplicitly]
internal class MenuInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<LevitatingNoteManager>().AsSingle();
        Container.BindInterfacesTo<SettingsMenuManager>().AsSingle();
    }
}