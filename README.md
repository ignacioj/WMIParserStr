# WMIParserStr

 WMI OBJECTS.DATA parser

Very fast. It can extract Consumers and EventFilters deleted or without a binding. These orphans are marked as TRUE in the last column of the report. False for the bindings and those Consumers and the EventConsumer that are binded.
 
-i Input file (OBJECTS.DATA)

-o Output directory for analysis results. Tab delimited file.

-s Ouput directory to save the strings (not Unicode) of OBJECTS.DATA




WMIParserStr.exe -i OBJECTS_A3.DATA -o .\ -s .\

##Console output:

Total Bindings: 22

[Binding]-[CommandLineEventConsumer]-[ConsumerA]-[Test]-[False]

[Binding]-[CommandLineEventConsumer]-[ConsumerA]-[Test]-[False]

[Binding]-[CommandLineEventConsumer]-[ConsumerA]-[Test]-[False]

[Binding]-[CommandLineEventConsumer]-[BotConsumer23]-[BotFilter82]-[False]

[Binding]-[CommandLineEventConsumer]-[ConsumerTest]-[Test]-[False]

[Binding]-[CommandLineEventConsumer]-[ConsumerA]-[Test]-[False]

[Binding]-[CommandLineEventConsumer]-[ConsumerA]-[Test]-[False]

[Binding]-[NTEventLogEventConsumer]-[SCM Event Log Consumer]-[SCM Event Log Filter]-[False]

...........

Total Consumers: 120

[CommandLineEventConsumer]-[CommandLineEventConsumer]-[__EventConsumerProviderRegistration]-[]-[True]

[CommandLineEventConsumer]-[InfectDrive]-[powershell.exe -NoP -C [Text.Encoding]::ASCII.GetString([Convert]::FromBase64String('WDVPIVAlQEFQWzRcUFpYNTQoUF4pN0NDKTd9JEVJQ0FSLVNUQU5EQVJELUFOVElWSVJVUy1URVNULUZJTEUhJEgrSCo=')) | Out-File %DriveName%\eicar.txt]-[]-[True]

[ActiveScriptEventConsumer]-[CleanupFileNames2]-[C:\fso\LaunchPowerShell.vbs]-[VBScript]-[True]

...........

Total EventFilters: 22

[__EventFilter]-[uint8]-[EventAccessstringEventNamespacestringNamestringQuery]-[CreatorSID]-[True]

[__EventFilter]-[uint8]-[EventAccessstringEventNamespacestringNamestringQuery]-[CreatorSID]-[True]

[__EventFilter]-[Test]-[SELECT * FROM __InstanceCreationEvent WITHIN 5 WHERE TargetInstance ISA 'CIM_DataFile'     AND TargetInstance.Drive = 'c:'     AND TargetInstance.Path = '\\test\\'     AND TargetInstance.Extension = 'txt']-[root\cimv2]-[False]

[__EventFilter]-[VolumeDetection]-[SELECT * FROM Win32_VolumeChangeEvent WHERE EventType=2]-[root\cimv2]-[False]

[__EventFilter]-[Backdoor Registry Filter]-[SELECT * FROM RegistryValueChangeEvent WHERE Hive='HKEY_LOCAL_MACHINE' AND KeyPath='SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\' AND ValueName = 'Registry Backdoor']-[root/cimv2]-[True]




##Tab delimited file:

Headers:       Type  ||     Name    ||             Content      ||          Other           ||                    Orphan

Elements:

Bindings:       Binding      ||        Type of Consumer  ||     Consumer name      ||       EventFilter name           ||           FALSE

Consumers:      Type of consumer   ||  Consumer name     ||     CommandLineTemplate   ||    [ExecutablePath][VBScript/JSCript]     ||   False/True 

EventFilter    __EventFilter   ||     EventFilter name   ||    Condition       ||          [root\cimv2][...]    ||  False/True

