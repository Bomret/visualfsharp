//
//Test #r with shadowcopy enabled
//
open System;;
open System.Diagnostics;;

let compiled wait =
    let c = Process.Start(Environment.GetEnvironmentVariable("FSC"), "Library1.fs --target:library")
    c.WaitForExit(wait) |> ignore
    if c.ExitCode = 0 then true else false;;

// Verify that System.Reflection.Assembly.GetEntryAssembly() does not return null
// When GetEntryAssembly is called inside an appdomain, if the assembly wasn't started useng ExecuteAssembly 
// GetEntryAssembly returns a null. this check ensures the appdomain is started the right way.
let verifyGetEntryAssembly = if System.Reflection.Assembly.GetEntryAssembly() = null then false else true

//  Build the library
let first = compiled 30000;;

//reference it will not lock the assembly because FSI was started with --shadowCopyReferences+
#r "Library1.dll";;

let next = compiled 30000;;
  
//compile will succeed because shadow copy is enabled
if next = true then
    printfn "Succeeded -- compile worked because file not locked due to --shadowcopyReferences+"
    if verifyGetEntryAssembly then
        printfn "Succeeded -- GetEntryAssembly() returned not null"
        use os = System.IO.File.CreateText "test2.ok" 
        os.Close()
    else
        printfn "Failed -- GetEntryAssembly() returned null"
else
    printfn "Failed -- compile failed because file was locked should not have been looked due to --shadowcopyReferences+";;
    
#quit;; 
