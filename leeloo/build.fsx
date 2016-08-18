#if INTERACTIVE
System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
#endif

#I "../packages/Leeloo/tools/"


#r "FakeLib.dll"
#r "Leeloo.dll"

open System
open Fake
open Fake.AssemblyInfoFile
open Leeloo

let version =
    let major = environVarOrDefault "LEELOO_MAJORVERSION" "1"
    let minor = environVarOrDefault "LEELOO_MINORVERSION" "1"    
    let build = environVarOrDefault "LEELOO_BUILDNUMBER"  "4"

    sprintf "%s.%s.%s" major minor build

let nuspecFileBase = "BeardedMonads.nuspec"

let projectRoot = ".."
let paths = new LeelooPaths(new System.IO.DirectoryInfo(projectRoot))

let buildFrameworks = [ V35; V4; V45; V451 ]

(* Use this to override which projects are built for frameworks *)
let shouldBuildForFramework (version: FrameworkVersion) (project: string) =
    match project, version with
//    | "ProjectName", v -> v >= V35 (* Builds "ProjectName" for any framework above 3.5 *)
    | _ -> true

(* Set your project name here *)
let nugetableProjects = ["Bearded.Monads"]

(* Use this to include dlls and pdbs from base libraries, e.g. a business or infrastructure layer. *)
let additionalBuildArtefacts = [
//    "SomeSharedLibrary" (* Will include the SomeSharedLibrary.dll and SomeSharedLibrary.pdb in the lib directory for the nuget package *)
]

(* Overrides for adding additional nuget references. Useful when projects are built as nuget packages *)
let specialisedRefs: MultipassTypes.SpecialisedReferencesMap = 
    function 
//    | "ProjectName" -> ["ChildProjectName", version] (* When building "ProjectName", an additional nuget reference will be added for ChildProjectName *)
    | _ -> []

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