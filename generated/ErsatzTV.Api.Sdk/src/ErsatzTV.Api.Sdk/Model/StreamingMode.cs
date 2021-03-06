/*
 * ErsatzTV API
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: v1
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using OpenAPIDateConverter = ErsatzTV.Api.Sdk.Client.OpenAPIDateConverter;

namespace ErsatzTV.Api.Sdk.Model
{
    /// <summary>
    /// Defines StreamingMode
    /// </summary>
    
    [JsonConverter(typeof(StringEnumConverter))]
    
    public enum StreamingMode
    {
        /// <summary>
        /// Enum TransportStream for value: TransportStream
        /// </summary>
        [EnumMember(Value = "TransportStream")]
        TransportStream = 1,

        /// <summary>
        /// Enum HttpLiveStreaming for value: HttpLiveStreaming
        /// </summary>
        [EnumMember(Value = "HttpLiveStreaming")]
        HttpLiveStreaming = 2

    }

}
