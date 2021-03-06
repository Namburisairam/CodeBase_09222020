#pragma checksum "H:\AutoIncidentTracking\Views\Incidents\AddEditES11Scripts.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "807647e6943542a5e9f18b18118eafbb6b1dc7c6"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Incidents_AddEditES11Scripts), @"mvc.1.0.view", @"/Views/Incidents/AddEditES11Scripts.cshtml")]
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
#nullable restore
#line 1 "H:\AutoIncidentTracking\Views\_ViewImports.cshtml"
using IncidentTracking;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "H:\AutoIncidentTracking\Views\_ViewImports.cshtml"
using IncidentTracking.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"807647e6943542a5e9f18b18118eafbb6b1dc7c6", @"/Views/Incidents/AddEditES11Scripts.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f795bc73ab22dff54ea51d3e61462dd07486b2b6", @"/Views/_ViewImports.cshtml")]
    public class Views_Incidents_AddEditES11Scripts : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(@"<script type=""text/javascript"">
    //helper function to show/hide conditional controls.
    function setConditionalControls() {
        var selectedActivityType = $('#ActivityTypeId').val();

        console.log(selectedActivityType);
        if (selectedActivityType === 'T' ||
            selectedActivityType === 'L' ||
            selectedActivityType === 'M') {
            console.log('pre-approval required');
            $('#RequirePreApproval').prop('checked', true);
            $('#RequireManagerApproval').prop('checked', true);
        }

        if (selectedActivityType === 'T') {
            $('#ImpactQuality').prop('checked', true);
        }

        if (selectedActivityType === 'S') {
            if ($('#ImpactSafety').is(':checked') || $('#ImpactQuality').is(':checked') || $('#ImpactValidation').is(':checked')) {
                console.log('pre-approval required by activity');
                $('#RequirePreApproval').prop('checked', true);
            }
        }
    }
");
            WriteLiteral(@"       
    //initially hide certain options
    $().ready(function () {
        $('#impacts').hide();
        $('#requirements').hide();
        $('#pre-approvals').hide();
        setConditionalControls();
    });
    
    $('#ActivityTypeId').change(function () {
        setConditionalControls();
        $('#impacts').show();
        $('#requirements').show();
        $('#pre-approvals').show();
    });
</script>
");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
