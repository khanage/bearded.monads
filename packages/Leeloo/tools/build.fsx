#if INTERACTIVE
System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
#endif

#I "../packages/Leeloo.1.1.13/tools/"


#r "FakeLib.dll"
#r "Leeloo.dll"

open System
open Fake
open Fake.AssemblyInfoFile
open Leeloo

let version =
    let major = environVarOrDefault "LEELOO_MAJORVERSION" "0"
    let minor = environVarOrDefault "LEELOO_MINORVERSION" "1"    
    let build = environVarOrDefault "LEELOO_BUILDNUMBER"  "0"

    sprintf "%s.%s.%s" major minor build

let nuspecFileBase = "sample.nuspec"

let projectRoot = ".."
let paths = new LeelooPaths(new System.IO.DirectoryInfo(projectRoot))

let buildFrameworks = [ V35 ; V451 ]

(* Use this to override which projects are built for frameworks *)
let shouldBuildForFramework (version: FrameworkVersion) (project: string) =
    match project, version with
//    | "ProjectName", v -> v >= V35 (* Builds "ProjectName" for any framework above 3.5 *)
    | _ -> true

(* ATTENTION: YOU MUST FOLLOW THE BELOW COMMENT *)
(*

    You must also select (by uncommenting) one of the following options:

        A) Single project - just take multiple passes over a single project.

        B) Single Interface, Many Implementations - use a convention based approach
            to easily build a base project which contains contracts and self contained logic
            and implementations for various technologies. This pattern is useful in that enables
            many packages to reference some common contract, and the application can then just provide 
            the implementation.
*)


(* A) Single project *)
//(* Set your project name here *)
//let nugetableProjects = ["<Your project name>"]
//
//(* Use this to include dlls and pdbs from base libraries, e.g. a business or infrastructure layer. *)
//let additionalBuildArtefacts = [
////    "SomeSharedLibrary" (* Will include the SomeSharedLibrary.dll and SomeSharedLibrary.pdb in the lib directory for the nuget package *)
//]
//
//(* Overrides for adding additional nuget references. Useful when projects are built as nuget packages *)
//let specialisedRefs: MultipassTypes.SpecialisedReferencesMap = 
//    function 
////    | "ProjectName" -> ["ChildProjectName", version] (* When building "ProjectName", an additional nuget reference will be added for ChildProjectName *)
//    | _ -> []


(* B) Build using interface+implementations convention *)
//(* Set your interface name here *)
//let interfaceProjectName = "<Your base *interface* project name>"
//
//(* 
//    Helper to determin any "derived" projects - the id can be replaced with a callback to change the parameters.
//
//    As an example, with the following projects
//
//    Logging              *
//    Logging.Tests        
//    Logging.NLog         *
//    Logging.Log4Net      *
//
//    All but the .Tests project will be returned for building        
//*)
//let nugetableProjects = interfaceProjectName |> InterfaceAndTechnologyPattern.projectsByConvention paths id
//
//(* Use this to include dlls and pdbs from base libraries, e.g. a business or infrastructure layer. *)
//let additionalBuildArtefacts = 
//    [
////    "SomeSharedLibrary" (* Will include the SomeSharedLibrary.dll and SomeSharedLibrary.pdb in the lib directory for the nuget package *)
//    ]
//
//(* This sets the interface as a nuget dependency for everything other than the interface itself *)
//let specialisedRefs: MultipassTypes.SpecialisedReferencesMap = 
//    function 
//    | p when p = interfaceProjectName -> []
////    | "ProjectName" -> ["ChildProjectName", version] (* When building "ProjectName", an additional nuget reference will be added for ChildProjectName *)
//    | _ -> [ interfaceProjectName, version ]


Target "Clean" (fun _ -> 
    Multipass.artefactDirectories paths
    |> List.iter CleanDir)

Target "UpdateSolutionInfo" (fun _ -> 
    let pathToSolutionInfo = paths.SourcesPath @@ "SolutionInfo.cs"
    CreateCSharpAssemblyInfo pathToSolutionInfo
        [ Attribute.Version version; Attribute.FileVersion version ])

Target "Build" (fun _ -> 
    nugetableProjects |> Multipass.copyAndBuildForAllFrameworks paths (fun a -> 
        { a with ShouldBuildForFramework = shouldBuildForFramework
                 Frameworks = buildFrameworks }))

Target "BuildTests" (fun _ -> 
    let testProjects = paths.SourcesPath @@ "*.Tests" @@ "*.csproj"

    !! testProjects
    |> MSBuildRelease paths.TestPath "Build"
    |> Log "Build tests ")

Target "Test" (fun _ -> 
    let testPattern = paths.TestPath @@ "*.Tests.dll"

    !! testPattern 
    |> NUnit(fun p -> 
            { p with DisableShadowCopy = true;
                        OutputFile = paths.BasePath @@ "TestResults.xml"
                        TimeOut = TimeSpan.FromMinutes 5. }))

Target "Nuget" (fun _ -> 
    let packageBuilder = Multipass.createNugetForProject paths (fun a -> 
        {a with FrameworksToBuild = buildFrameworks
                NuspecTemplatePath = nuspecFileBase                
                ShouldBuildForFramework = shouldBuildForFramework
                SpecialisedReferences = specialisedRefs
                IncludeOutputFromProjectsNamed = additionalBuildArtefacts
                Version = version})
        
    nugetableProjects |> Seq.iter packageBuilder)

Target "Default" (fun _ -> 
    trace <| "Build complete.")

"Clean" 
==> "UpdateSolutionInfo" 
==> "Build"
==> "BuildTests"
==> "Test"
==> "Nuget"
==> "Default"

RunTargetOrDefault "Default"