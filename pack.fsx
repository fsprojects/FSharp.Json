#!/bin/dotnet fsi

#r "nuget: Fake.Core.ReleaseNotes"
#r "nuget: Fake.DotNet.Cli"

// Settings
let projectName = "FSharp.Json"
let nugetDir = "./out"

let authors =
    [ "vsapronov"; "NicoVIII" ] |> String.concat ";"

let summary =
    "F# JSON Reflection based serialization library"

let tags = "F# JSON serialization"
let copyright = "Copyright 2019"
let license = "Apache-2.0"

let gitOwner = "NicoVIII"
let gitName = projectName
let gitHome = "https://github.com/" + gitOwner
let gitUrl = gitHome + "/" + gitName

module Changelog =
    open Fake.Core

    let changelogFilename = "CHANGELOG.md"
    let changelog = Changelog.load changelogFilename
    let latestEntry = changelog.LatestEntry

    let nugetVersion = latestEntry.NuGetVersion

    let packageReleaseNotes =
        sprintf "%s/blob/v%s/CHANGELOG.md" gitUrl nugetVersion

module Pack =
    open Fake.DotNet

    let options (options: DotNet.PackOptions) =
        let properties =
            [ ("Version", Changelog.nugetVersion)
              ("Authors", authors)
              ("PackageProjectUrl", gitUrl)
              ("PackageTags", tags)
              ("RepositoryType", "git")
              ("RepositoryUrl", gitUrl)
              ("PackageLicenseExpression", license)
              ("Copyright", copyright)
              ("PackageDescription", summary)
              ("PackageReleaseNotes", Changelog.packageReleaseNotes)
              ("EnableSourceLink", "true") ]

        { options with
              OutputPath = Some nugetDir
              MSBuildParams =
                  { options.MSBuildParams with
                        Properties = properties } }

    let execute () =
        DotNet.pack options $"{projectName}/{projectName}.fsproj"

Pack.execute ()
