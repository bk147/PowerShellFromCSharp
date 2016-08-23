#
# New-ScriptFileInfo
# New-Module
# New-ModuleManifest
#

<#
.Synopsis
   Short description
.DESCRIPTION
   Long description
.EXAMPLE
   Example of how to use this cmdlet
.EXAMPLE
   Another example of how to use this cmdlet
#>
function New-User
{
    [CmdletBinding()]
    [Alias()]
    [OutputType([string])]
    Param
    (
        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   Position=0)]
        [string]$UserName,

        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   Position=1)]
        [string]$Firstname,

        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   Position=2)]
        [string]$Lastname,

        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   Position=3)]
        [int]$Uid,

        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   Position=4)]
        [string]$Domain,

        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   Position=5)]
        [string]$Password,

        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   Position=6)]
        [string]$Type,

        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   Position=7)]
        [string]$Workgroup
    )

    Begin
    {
        $logpath = 'C:\_tmp\Eduser.txt'
    }
    Process
    {
        "<--------New-User--------->" | Out-File -FilePath $logpath -Append
        [datetime]::Now | Out-File -FilePath $logpath -Append
        $UserName | Out-File -FilePath $logpath -Append
        $Firstname | Out-File -FilePath $logpath -Append
        $Lastname | Out-File -FilePath $logpath -Append
        $Uid | Out-File -FilePath $logpath -Append
        $Domain | Out-File -FilePath $logpath -Append
        $Password | Out-File -FilePath $logpath -Append
        $Type | Out-File -FilePath $logpath -Append
        $Workgroup | Out-File -FilePath $logpath -Append
    }
    End
    {
    }
}

<#
.Synopsis
   Short description
.DESCRIPTION
   Long description
.EXAMPLE
   Example of how to use this cmdlet
.EXAMPLE
   Another example of how to use this cmdlet
#>
function Test01
{
    [CmdletBinding()]
    [Alias()]
    [OutputType([string])]
    Param
    (
        # Param1 help description
        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   Position=0)]
        $Param1,

        # Param2 help description
        [Parameter(Mandatory=$true,
                   ValueFromPipelineByPropertyName=$true,
                   Position=1)]
        [int]
        $Param2
    )

    Begin
    {
        $logpath = 'C:\_tmp\Eduser.txt'
    }
    Process
    {
         "<--------Test01--------->" | Out-File -FilePath $logpath -Append
        [datetime]::Now | Out-File -FilePath $logpath -Append
        $Param1 | Out-File -FilePath $logpath -Append
        $Param2 | Out-File -FilePath $logpath -Append
   }
    End
    {
    }
}