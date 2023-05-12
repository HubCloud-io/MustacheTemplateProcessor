﻿namespace MustacheTemplateProcessor;

public interface IMustacheParser
{
    string Parse(string expression, Dictionary<string, object> context);
}