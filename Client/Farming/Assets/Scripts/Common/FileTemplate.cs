using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileTemplate : SingleTon<FileTemplate>
{
    public string protoTemplate = "syntax = \"proto2\";\n\n" +
        "package config;\n\n" +
        "option optimize_for = LITE_RUNTIME;\n\n" +
        "message ${TABLE_NAME}\n" +
        "{\n" +
        "\tmessage ${RECORD_NAME}\n" +
        "\t{\n" +
        "${RECORD_LIST}\n" +
        "\t}\n" +
        "\trepeated ${RECORD_NAME} ${RECORDS_VAR_NAME} = 1;\n" +
        "${ATTRIBUTE_LIST}\n" +
        "}";
}
