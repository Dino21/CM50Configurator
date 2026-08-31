; Script Inno Setup per CM50 Configurator.
; Per ricompilare l'installer dopo una modifica al codice:
;   1) Compilare la soluzione in configurazione Release (bin\Release deve essere aggiornato).
;   2) Aprire questo file con Inno Setup Compiler (o eseguire ISCC.exe CM50-Configurator.iss).
; L'output (CM50-Configurator-Setup-<versione>.exe) viene generato in installer\Output.

#define MyAppName "CM50 Configurator"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Sesami"
#define MyAppExeName "CM50-Configurator.exe"
#define MyReleaseDir "..\CM50-Configurator\bin\Release"

[Setup]
; Stesso GUID del ProjectGuid della soluzione: identifica in modo stabile l'app tra una
; versione e l'altra dell'installer, per permettere aggiornamenti in-place.
AppId={{5E05334E-4D31-43B8-A4AA-C1D2F66C7726}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName=C:\{#MyAppPublisher}\{#MyAppName}
DefaultGroupName={#MyAppPublisher}\{#MyAppName}
UninstallDisplayIcon={app}\{#MyAppExeName}
OutputDir=Output
OutputBaseFilename=CM50-Configurator-Setup-{#MyAppVersion}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
SetupIconFile=..\CM50-Configurator\Assets\sesami-app-icon.ico
; Permette sia l'installazione per tutti gli utenti (richiede privilegi amministrativi)
; sia quella per il solo utente corrente (nessun privilegio richiesto), a scelta di chi
; esegue l'installer.
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=commandline dialog
ArchitecturesInstallIn64BitMode=x64compatible
DisableProgramGroupPage=yes

[Languages]
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"

[Tasks]
Name: "desktopicon"; Description: "Crea un'icona sul desktop"; GroupDescription: "Icone aggiuntive:"; Flags: unchecked

[Files]
; Tutto l'output di build Release: l'eseguibile, App.config (rinominato in .exe.config),
; le DLL di MySqlConnector/Newtonsoft.Json e le loro dipendenze transitive. Il .pdb (debug
; symbols) viene escluso: non serve per l'uso normale dell'applicazione.
Source: "{#MyReleaseDir}\*"; DestDir: "{app}"; Excludes: "*.pdb"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Disinstalla {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Avvia {#MyAppName}"; Flags: nowait postinstall skipifsilent
