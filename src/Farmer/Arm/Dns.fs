[<AutoOpen>]
module Farmer.Arm.Dns

open Farmer
open Farmer.CoreTypes
open Farmer.Dns

let zones = ResourceType ("Microsoft.Network/dnszones", "2018-05-01")
let aRecord = ResourceType ("Microsoft.Network/dnszones/A", "2018-05-01")
let aaaaRecord = ResourceType ("Microsoft.Network/dnszones/AAAA", "2018-05-01")
let cnameRecord = ResourceType ("Microsoft.Network/dnszones/CNAME", "2018-05-01")
let txtRecord = ResourceType ("Microsoft.Network/dnszones/TXT", "2018-05-01")
let mxRecord = ResourceType ("Microsoft.Network/dnszones/MX", "2018-05-01")
let nsRecord = ResourceType ("Microsoft.Network/dnszones/NS", "2018-05-01")
let soaRecord = ResourceType ("Microsoft.Network/dnszones/SOA", "2018-05-01")
let srvRecord = ResourceType ("Microsoft.Network/dnszones/SRV", "2018-05-01")
let ptrRecord = ResourceType ("Microsoft.Network/dnszones/PTR", "2018-05-01")

type DnsRecordType with
    member this.ResourceType =
        match this with
        | Unknown -> failwith "Not Implemented"
        | CName -> cnameRecord
        | A -> aRecord
        | AAAA -> aaaaRecord
        | NS -> nsRecord
        | PTR -> ptrRecord
        | TXT -> txtRecord
        | MX -> mxRecord
        // | Soa -> soaRecord
        // | Srv -> srvRecord

type DnsZone =
    { Name : ResourceName
      Properties : {| ZoneType : string |} }

    interface IArmResource with
        member this.ResourceName = this.Name
        member this.JsonModel =
            {| zones.Create(this.Name, Location.Global) with
                properties = {| zoneType = this.Properties.ZoneType |}
            |} :> _

module DnsRecords =
    type CNameDnsRecord =
        { Name : ResourceName
          Zone : ResourceName
          Type : DnsRecordType
          TTL : int
          TargetResource : ResourceName option
          CNameRecord : string option }
        interface IArmResource with
            member this.ResourceName = this.Name
            member this.JsonModel =
                {| this.Type.ResourceType.Create(this.Zone + this.Name, dependsOn = [ this.Zone ]) with
                    properties =
                        {| TTL = this.TTL
                           targetResource = {| id = this.TargetResource |}
                           CNAMERecords = {| cname = this.CNameRecord |> Option.toObj |} |}
                |} :> _

    type MxDnsRecord =
        { Name : ResourceName
          Zone : ResourceName
          Type : DnsRecordType
          TTL : int
          MxRecords : {| Preference : int; Exchange : string |} list }
        interface IArmResource with
            member this.ResourceName = this.Name
            member this.JsonModel =
                {| this.Type.ResourceType.Create(this.Zone + this.Name, dependsOn = [ this.Zone ]) with
                    properties =
                        {| TTL = this.TTL;
                           MXRecords = this.MxRecords |> List.map (fun mx -> {| preference = mx.Preference; exchange = mx.Exchange |}) |}
                |} :> _

    type NsDnsRecord =
        { Name : ResourceName
          Zone : ResourceName
          Type : DnsRecordType
          TTL : int
          NsRecords : string list }
        interface IArmResource with
            member this.ResourceName = this.Name
            member this.JsonModel =
                {| this.Type.ResourceType.Create(this.Zone + this.Name, dependsOn = [ this.Zone ]) with
                    properties =
                        {| TTL = this.TTL
                           targetResource = {| id = null |}
                           NSRecords = this.NsRecords |> List.map (fun ns -> {| nsdname = ns |}) |}
                |} :> _

    type TxtDnsRecord =
        { Name : ResourceName
          Zone : ResourceName
          Type : DnsRecordType
          TTL : int
          TxtRecords : string list }
        interface IArmResource with
            member this.ResourceName = this.Name
            member this.JsonModel =
                {| this.Type.ResourceType.Create(this.Zone + this.Name, dependsOn = [ this.Zone ]) with
                    properties =
                        {| TTL = this.TTL;
                           TXTRecords = this.TxtRecords |> List.map (fun txt -> {| value = [ txt ] |}) |}
                |} :> _

    type PtrDnsRecord =
        { Name : ResourceName
          Zone : ResourceName
          Type : DnsRecordType
          TTL : int
          PtrRecords : string list }
        interface IArmResource with
            member this.ResourceName = this.Name
            member this.JsonModel =
                {| this.Type.ResourceType.Create(this.Zone + this.Name, dependsOn = [ this.Zone ]) with
                    properties =
                        {| TTL = this.TTL;
                           PTRRecords = this.PtrRecords |> List.map (fun ptr -> {| ptrdname = ptr |}) |}
                |} :> _

    type ADnsRecord =
        { Name : ResourceName
          Zone : ResourceName
          Type : DnsRecordType
          TTL : int
          TargetResource : ResourceName option
          ARecords : string list }
        interface IArmResource with
            member this.ResourceName = this.Name
            member this.JsonModel =
                {| this.Type.ResourceType.Create(this.Zone + this.Name, dependsOn = [ this.Zone ]) with
                    properties =
                        {| TTL = this.TTL;
                           targetResource = {| id = this.TargetResource |}
                           ARecords = this.ARecords |> List.map (fun a -> {| ipv4Address = a |}) |}
                |} :> _

    type AaaaDnsRecord =
        { Name : ResourceName
          Zone : ResourceName
          Type : DnsRecordType
          TTL : int
          TargetResource : ResourceName option
          AaaaRecords : string list }
        interface IArmResource with
            member this.ResourceName = this.Name
            member this.JsonModel =
                {| this.Type.ResourceType.Create(this.Zone + this.Name, dependsOn = [ this.Zone ]) with
                    properties =
                        {| TTL = this.TTL;
                           targetResource = {| id = this.TargetResource |}
                           AAAARecords = this.AaaaRecords |> List.map (fun aaaa -> {| ipv6Address = aaaa |}) |}
                |} :> _