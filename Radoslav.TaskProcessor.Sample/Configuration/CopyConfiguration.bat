cd %~dp0

copy "Radoslav.TaskProcessor.ServiceLocator.config" "..\..\Radoslav.TaskProcessor\bin\%1\Radoslav.TaskProcessor.ServiceLocator.config"
copy "Radoslav.TaskProcessor.Serialization.config" "..\..\Radoslav.TaskProcessor\bin\%1\Radoslav.TaskProcessor.Serialization.config"
copy "Radoslav.TaskProcessor.config" "..\..\Radoslav.TaskProcessor\bin\%1\Radoslav.TaskProcessor.Monitor.ConsoleApp.exe.config"
copy "Radoslav.TaskProcessor.config" "..\..\Radoslav.TaskProcessor\bin\%1\Radoslav.TaskProcessor.WindowsService.exe.config"
copy "Radoslav.TaskProcessor.TaskWorker.config" "..\..\Radoslav.TaskProcessor\bin\%1\Radoslav.TaskProcessor.TaskWorker.exe.config"
copy "Radoslav.TaskProcessor.Monitor.Wpf.config" "..\..\Radoslav.TaskProcessor\bin\%1\Radoslav.TaskProcessor.Monitor.Wpf.exe.config"

pause