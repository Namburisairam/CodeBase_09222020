#pragma checksum "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "2f35e7e41b7aaecfd2f4c548ef6428ec65e49d4a"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Incidents_DetailsPartialView), @"mvc.1.0.view", @"/Views/Incidents/DetailsPartialView.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Incidents/DetailsPartialView.cshtml", typeof(AspNetCore.Views_Incidents_DetailsPartialView))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "H:\AutoIncidentTracking\Views\_ViewImports.cshtml"
using IncidentTracking;

#line default
#line hidden
#line 2 "H:\AutoIncidentTracking\Views\_ViewImports.cshtml"
using IncidentTracking.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"2f35e7e41b7aaecfd2f4c548ef6428ec65e49d4a", @"/Views/Incidents/DetailsPartialView.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f795bc73ab22dff54ea51d3e61462dd07486b2b6", @"/Views/_ViewImports.cshtml")]
    public class Views_Incidents_DetailsPartialView : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IncidentTracking.Models.Incident>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(914, 364, true);
            WriteLiteral(@"

<h2>Incident Details</h2>

<hr />

<style>
    table {
        border: none;
        border-collapse: collapse;
    }

    th {
        font-weight: bold;
        text-align: right;
        border: none;
        padding-right: 10px;
    }

    td { 
        border: none; 

    }

</style>

<table>
    <tr>
        <th>
            ");
            EndContext();
            BeginContext(1279, 56, false);
#line 50 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayNameFor(model => model.LocalCreatedDateTime));

#line default
#line hidden
            EndContext();
            BeginContext(1335, 44, true);
            WriteLiteral(":\r\n        </th>\r\n        <td>\r\n            ");
            EndContext();
            BeginContext(1380, 52, false);
#line 53 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayFor(model => model.LocalCreatedDateTime));

#line default
#line hidden
            EndContext();
            BeginContext(1432, 66, true);
            WriteLiteral("\r\n        </td>\r\n    </tr>\r\n    <tr>\r\n\r\n        <th>\r\n            ");
            EndContext();
            BeginContext(1499, 56, false);
#line 59 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayNameFor(model => model.LastModifiedDateTime));

#line default
#line hidden
            EndContext();
            BeginContext(1555, 44, true);
            WriteLiteral(":\r\n        </th>\r\n        <td>\r\n            ");
            EndContext();
            BeginContext(1600, 57, false);
#line 62 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayFor(model => model.LocalLastModifiedDateTime));

#line default
#line hidden
            EndContext();
            BeginContext(1657, 64, true);
            WriteLiteral("\r\n        </td>\r\n    </tr>\r\n    <tr>\r\n        <th>\r\n            ");
            EndContext();
            BeginContext(1722, 48, false);
#line 67 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayNameFor(model => model.EngineerName));

#line default
#line hidden
            EndContext();
            BeginContext(1770, 44, true);
            WriteLiteral(":\r\n        </th>\r\n        <td>\r\n            ");
            EndContext();
            BeginContext(1815, 44, false);
#line 70 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayFor(model => model.EngineerName));

#line default
#line hidden
            EndContext();
            BeginContext(1859, 64, true);
            WriteLiteral("\r\n        </td>\r\n    </tr>\r\n    <tr>\r\n        <th>\r\n            ");
            EndContext();
            BeginContext(1924, 52, false);
#line 75 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayNameFor(model => model.ShortDescription));

#line default
#line hidden
            EndContext();
            BeginContext(1976, 44, true);
            WriteLiteral(":\r\n        </th>\r\n        <td>\r\n            ");
            EndContext();
            BeginContext(2021, 48, false);
#line 78 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayFor(model => model.ShortDescription));

#line default
#line hidden
            EndContext();
            BeginContext(2069, 104, true);
            WriteLiteral("\r\n        </td>\r\n    </tr>\r\n    <tr>\r\n\r\n        <th>Manufacturing Area:</th>\r\n        <td>\r\n            ");
            EndContext();
            BeginContext(2174, 54, false);
#line 85 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayFor(model => model.ManufacturingArea.Name));

#line default
#line hidden
            EndContext();
            BeginContext(2228, 66, true);
            WriteLiteral("\r\n        </td>\r\n    </tr>\r\n    <tr>\r\n\r\n        <th>\r\n            ");
            EndContext();
            BeginContext(2295, 74, false);
#line 91 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayNameFor(model => model.ControlSystem.ControlSystemDescription));

#line default
#line hidden
            EndContext();
            BeginContext(2369, 44, true);
            WriteLiteral(":\r\n        </th>\r\n        <td>\r\n            ");
            EndContext();
            BeginContext(2414, 70, false);
#line 94 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayFor(model => model.ControlSystem.ControlSystemDescription));

#line default
#line hidden
            EndContext();
            BeginContext(2484, 66, true);
            WriteLiteral("\r\n        </td>\r\n    </tr>\r\n    <tr>\r\n\r\n        <th>\r\n            ");
            EndContext();
            BeginContext(2551, 47, false);
#line 100 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayNameFor(model => model.AreaContact));

#line default
#line hidden
            EndContext();
            BeginContext(2598, 44, true);
            WriteLiteral(":\r\n        </th>\r\n        <td>\r\n            ");
            EndContext();
            BeginContext(2643, 43, false);
#line 103 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayFor(model => model.AreaContact));

#line default
#line hidden
            EndContext();
            BeginContext(2686, 66, true);
            WriteLiteral("\r\n        </td>\r\n    </tr>\r\n    <tr>\r\n\r\n        <th>\r\n            ");
            EndContext();
            BeginContext(2753, 58, false);
#line 109 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayNameFor(model => model.EstimatedDownTimeHours));

#line default
#line hidden
            EndContext();
            BeginContext(2811, 44, true);
            WriteLiteral(":\r\n        </th>\r\n        <td>\r\n            ");
            EndContext();
            BeginContext(2856, 54, false);
#line 112 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayFor(model => model.EstimatedDownTimeHours));

#line default
#line hidden
            EndContext();
            BeginContext(2910, 100, true);
            WriteLiteral("\r\n        </td>\r\n    </tr>\r\n    <tr>\r\n\r\n        <th>Classification:</th>\r\n        <td>\r\n            ");
            EndContext();
            BeginContext(3011, 51, false);
#line 119 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayFor(model => model.Classification.Name));

#line default
#line hidden
            EndContext();
            BeginContext(3062, 66, true);
            WriteLiteral("\r\n        </td>\r\n    </tr>\r\n    <tr>\r\n\r\n        <th>\r\n            ");
            EndContext();
            BeginContext(3129, 44, false);
#line 125 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayNameFor(model => model.Resolved));

#line default
#line hidden
            EndContext();
            BeginContext(3173, 44, true);
            WriteLiteral(":\r\n        </th>\r\n        <td>\r\n            ");
            EndContext();
            BeginContext(3219, 62, false);
#line 128 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
        Write(Model.Resolved.HasValue && Model.Resolved.Value ? "Yes" : "No");

#line default
#line hidden
            EndContext();
            BeginContext(3282, 64, true);
            WriteLiteral("\r\n        </td>\r\n    </tr>\r\n    <tr>\r\n        <th>\r\n            ");
            EndContext();
            BeginContext(3347, 47, false);
#line 133 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayNameFor(model => model.HandoffWork));

#line default
#line hidden
            EndContext();
            BeginContext(3394, 44, true);
            WriteLiteral(":\r\n        </th>\r\n        <td>\r\n            ");
            EndContext();
            BeginContext(3439, 43, false);
#line 136 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayFor(model => model.HandoffWork));

#line default
#line hidden
            EndContext();
            BeginContext(3482, 64, true);
            WriteLiteral("\r\n        </td>\r\n    </tr>\r\n    <tr>\r\n        <th>\r\n            ");
            EndContext();
            BeginContext(3547, 47, false);
#line 141 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayNameFor(model => model.RequireES11));

#line default
#line hidden
            EndContext();
            BeginContext(3594, 44, true);
            WriteLiteral(":\r\n        </th>\r\n        <td>\r\n            ");
            EndContext();
            BeginContext(3640, 68, false);
#line 144 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
        Write(Model.RequireES11.HasValue && Model.RequireES11.Value ? "Yes" : "No");

#line default
#line hidden
            EndContext();
            BeginContext(3709, 64, true);
            WriteLiteral("\r\n        </td>\r\n    </tr>\r\n    <tr>\r\n        <th>\r\n            ");
            EndContext();
            BeginContext(3774, 55, false);
#line 149 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayNameFor(model => model.IncidentDescription));

#line default
#line hidden
            EndContext();
            BeginContext(3829, 44, true);
            WriteLiteral(":\r\n        </th>\r\n        <td>\r\n            ");
            EndContext();
            BeginContext(3874, 51, false);
#line 152 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayFor(model => model.IncidentDescription));

#line default
#line hidden
            EndContext();
            BeginContext(3925, 64, true);
            WriteLiteral("\r\n        </td>\r\n    </tr>\r\n    <tr>\r\n        <th>\r\n            ");
            EndContext();
            BeginContext(3990, 49, false);
#line 157 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayNameFor(model => model.ActionSummary));

#line default
#line hidden
            EndContext();
            BeginContext(4039, 44, true);
            WriteLiteral(":\r\n        </th>\r\n        <td>\r\n            ");
            EndContext();
            BeginContext(4084, 45, false);
#line 160 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
       Write(Html.DisplayFor(model => model.ActionSummary));

#line default
#line hidden
            EndContext();
            BeginContext(4129, 30, true);
            WriteLiteral("\r\n        </td>\r\n    </tr>\r\n\r\n");
            EndContext();
#line 164 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
     if (Model.RequireES11.HasValue && Model.RequireES11.Value)
    {

#line default
#line hidden
            BeginContext(4231, 82, true);
            WriteLiteral("        <tr>\r\n            <th>Log Number:</th>\r\n            <td>\r\n                ");
            EndContext();
            BeginContext(4314, 41, false);
#line 169 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayFor(model => model.LogNumber));

#line default
#line hidden
            EndContext();
            BeginContext(4355, 84, true);
            WriteLiteral("\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <th>\r\n                ");
            EndContext();
            BeginContext(4440, 50, false);
#line 174 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayNameFor(model => model.ActivityTypeId));

#line default
#line hidden
            EndContext();
            BeginContext(4490, 56, true);
            WriteLiteral(":\r\n            </th>\r\n            <td>\r\n                ");
            EndContext();
            BeginContext(4547, 107, false);
#line 177 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.Label("ActivityType.Description", Model.ActivityType == null ? "N/A" : Model.ActivityType.Description));

#line default
#line hidden
            EndContext();
            BeginContext(4654, 84, true);
            WriteLiteral("\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <th>\r\n                ");
            EndContext();
            BeginContext(4739, 48, false);
#line 182 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayNameFor(model => model.ImpactSafety));

#line default
#line hidden
            EndContext();
            BeginContext(4787, 56, true);
            WriteLiteral(":\r\n            </th>\r\n            <td>\r\n                ");
            EndContext();
            BeginContext(4845, 70, false);
#line 185 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
            Write(Model.ImpactSafety.HasValue && Model.ImpactSafety.Value ? "Yes" : "No");

#line default
#line hidden
            EndContext();
            BeginContext(4916, 84, true);
            WriteLiteral("\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <th>\r\n                ");
            EndContext();
            BeginContext(5001, 49, false);
#line 190 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayNameFor(model => model.ImpactQuality));

#line default
#line hidden
            EndContext();
            BeginContext(5050, 56, true);
            WriteLiteral(":\r\n            </th>\r\n            <td>\r\n                ");
            EndContext();
            BeginContext(5108, 72, false);
#line 193 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
            Write(Model.ImpactQuality.HasValue && Model.ImpactQuality.Value ? "Yes" : "No");

#line default
#line hidden
            EndContext();
            BeginContext(5181, 84, true);
            WriteLiteral("\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <th>\r\n                ");
            EndContext();
            BeginContext(5266, 52, false);
#line 198 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayNameFor(model => model.ImpactValidation));

#line default
#line hidden
            EndContext();
            BeginContext(5318, 56, true);
            WriteLiteral(":\r\n            </th>\r\n            <td>\r\n                ");
            EndContext();
            BeginContext(5376, 78, false);
#line 201 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
            Write(Model.ImpactValidation.HasValue && Model.ImpactValidation.Value ? "Yes" : "No");

#line default
#line hidden
            EndContext();
            BeginContext(5455, 84, true);
            WriteLiteral("\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <th>\r\n                ");
            EndContext();
            BeginContext(5540, 54, false);
#line 206 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayNameFor(model => model.RequirePreApproval));

#line default
#line hidden
            EndContext();
            BeginContext(5594, 56, true);
            WriteLiteral(":\r\n            </th>\r\n            <td>\r\n                ");
            EndContext();
            BeginContext(5652, 82, false);
#line 209 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
            Write(Model.RequirePreApproval.HasValue && Model.RequirePreApproval.Value ? "Yes" : "No");

#line default
#line hidden
            EndContext();
            BeginContext(5735, 84, true);
            WriteLiteral("\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <th>\r\n                ");
            EndContext();
            BeginContext(5820, 58, false);
#line 214 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayNameFor(model => model.RequireManagerApproval));

#line default
#line hidden
            EndContext();
            BeginContext(5878, 56, true);
            WriteLiteral(":\r\n            </th>\r\n            <td>\r\n                ");
            EndContext();
            BeginContext(5936, 90, false);
#line 217 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
            Write(Model.RequireManagerApproval.HasValue && Model.RequireManagerApproval.Value ? "Yes" : "No");

#line default
#line hidden
            EndContext();
            BeginContext(6027, 84, true);
            WriteLiteral("\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <th>\r\n                ");
            EndContext();
            BeginContext(6112, 49, false);
#line 222 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayNameFor(model => model.RequireTagOut));

#line default
#line hidden
            EndContext();
            BeginContext(6161, 56, true);
            WriteLiteral(":\r\n            </th>\r\n            <td>\r\n                ");
            EndContext();
            BeginContext(6219, 72, false);
#line 225 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
            Write(Model.RequireTagOut.HasValue && Model.RequireTagOut.Value ? "Yes" : "No");

#line default
#line hidden
            EndContext();
            BeginContext(6292, 84, true);
            WriteLiteral("\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <th>\r\n                ");
            EndContext();
            BeginContext(6377, 44, false);
#line 230 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayNameFor(model => model.PRNumber));

#line default
#line hidden
            EndContext();
            BeginContext(6421, 56, true);
            WriteLiteral(":\r\n            </th>\r\n            <td>\r\n                ");
            EndContext();
            BeginContext(6478, 40, false);
#line 233 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayFor(model => model.PRNumber));

#line default
#line hidden
            EndContext();
            BeginContext(6518, 84, true);
            WriteLiteral("\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <th>\r\n                ");
            EndContext();
            BeginContext(6603, 48, false);
#line 238 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayNameFor(model => model.TagOutNumber));

#line default
#line hidden
            EndContext();
            BeginContext(6651, 56, true);
            WriteLiteral(":\r\n            </th>\r\n            <td>\r\n                ");
            EndContext();
            BeginContext(6708, 44, false);
#line 241 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayFor(model => model.TagOutNumber));

#line default
#line hidden
            EndContext();
            BeginContext(6752, 84, true);
            WriteLiteral("\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <th>\r\n                ");
            EndContext();
            BeginContext(6837, 56, false);
#line 246 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayNameFor(model => model.SystemSpecifications));

#line default
#line hidden
            EndContext();
            BeginContext(6893, 56, true);
            WriteLiteral(":\r\n            </th>\r\n            <td>\r\n                ");
            EndContext();
            BeginContext(6950, 52, false);
#line 249 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayFor(model => model.SystemSpecifications));

#line default
#line hidden
            EndContext();
            BeginContext(7002, 84, true);
            WriteLiteral("\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <th>\r\n                ");
            EndContext();
            BeginContext(7087, 44, false);
#line 254 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayNameFor(model => model.Drawings));

#line default
#line hidden
            EndContext();
            BeginContext(7131, 56, true);
            WriteLiteral(":\r\n            </th>\r\n            <td>\r\n                ");
            EndContext();
            BeginContext(7188, 40, false);
#line 257 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayFor(model => model.Drawings));

#line default
#line hidden
            EndContext();
            BeginContext(7228, 84, true);
            WriteLiteral("\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <th>\r\n                ");
            EndContext();
            BeginContext(7313, 45, false);
#line 262 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayNameFor(model => model.LotNumber));

#line default
#line hidden
            EndContext();
            BeginContext(7358, 56, true);
            WriteLiteral(":\r\n            </th>\r\n            <td>\r\n                ");
            EndContext();
            BeginContext(7415, 41, false);
#line 265 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayFor(model => model.LotNumber));

#line default
#line hidden
            EndContext();
            BeginContext(7456, 84, true);
            WriteLiteral("\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <th>\r\n                ");
            EndContext();
            BeginContext(7541, 57, false);
#line 270 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayNameFor(model => model.ProgramComparisonMade));

#line default
#line hidden
            EndContext();
            BeginContext(7598, 56, true);
            WriteLiteral(":\r\n            </th>\r\n            <td>\r\n                ");
            EndContext();
            BeginContext(7656, 88, false);
#line 273 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
            Write(Model.ProgramComparisonMade.HasValue && Model.ProgramComparisonMade.Value ? "Yes" : "No");

#line default
#line hidden
            EndContext();
            BeginContext(7745, 84, true);
            WriteLiteral("\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <th>\r\n                ");
            EndContext();
            BeginContext(7830, 52, false);
#line 278 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
           Write(Html.DisplayNameFor(model => model.ActivityComplete));

#line default
#line hidden
            EndContext();
            BeginContext(7882, 56, true);
            WriteLiteral(":\r\n            </th>\r\n            <td>\r\n                ");
            EndContext();
            BeginContext(7940, 78, false);
#line 281 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
            Write(Model.ActivityComplete.HasValue && Model.ActivityComplete.Value ? "Yes" : "No");

#line default
#line hidden
            EndContext();
            BeginContext(8019, 40, true);
            WriteLiteral("\r\n            </td>\r\n        </tr>    \r\n");
            EndContext();
#line 284 "H:\AutoIncidentTracking\Views\Incidents\DetailsPartialView.cshtml"
     }

#line default
#line hidden
            BeginContext(8067, 10, true);
            WriteLiteral("\r\n</table>");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IncidentTracking.Models.Incident> Html { get; private set; }
    }
}
#pragma warning restore 1591