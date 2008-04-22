using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Linq.TreeMasher;
using System.Linq.Expressions;
using System.Reflection;

namespace LinFu.Persist
{
    //public class SimpleLogicalComparison : ExpressionVisitor
    //{
    //    private static Dictionary<ExpressionType, string> _expressionMap = new Dictionary<ExpressionType, string>();

    //    static SimpleLogicalComparison()
    //    {
    //        _expressionMap[ExpressionType.Equal] = "=";
    //        _expressionMap[ExpressionType.GreaterThan] = ">";
    //        _expressionMap[ExpressionType.GreaterThanOrEqual] = ">=";
    //        _expressionMap[ExpressionType.LessThan] = "<";
    //        _expressionMap[ExpressionType.LessThanOrEqual] = "<=";
    //    }
    //    public SimpleLogicalComparison(StringBuilder builder)
    //    {
    //        this.Builder = builder;
    //    }

    //    public StringBuilder Builder { get; set; }
    //    public IPropertyMappingRegistry PropertyMappingRegistry
    //    {
    //        get;
    //        set;
    //    }
    //    public override void Visit(IMetaExpression expression)
    //    {
    //        if (Builder == null)
    //            return;

    //        Visit(Builder, expression);
    //    }
    //    public override bool Matches(IMetaExpression expression)
    //    {
    //        if (PropertyMappingRegistry == null)
    //            return false;

    //        if (expression.ExpressionType != typeof(BinaryExpression))
    //            return false;

    //        if (!expression.TargetExpression.IsLogicalComparison())
    //            return false;

    //        // NOTE: Only the following expression syntax is supported:
    //        // PropertyValue 'operator' constant
    //        if (expression.Children.Count != 2)
    //            return false;

    //        // The left child must be a MemberExpression
    //        var leftChild = expression.Children[0];
    //        if (leftChild.ExpressionType != typeof(MemberExpression))
    //            return false;

    //        // The right child must be a constant
    //        var rightChild = expression.Children[1];
    //        if (rightChild.ExpressionType != typeof(ConstantExpression))
    //            return false;

    //        // The property must map to a field that exists in the
    //        // target table             
    //        var targetProperty = ExtractProperty(leftChild);
    //        if (targetProperty == null)
    //            return false;

    //        // The property must be a scalar value type
    //        if (!targetProperty.PropertyType.IsValueType && targetProperty.PropertyType != typeof(string))
    //            return false;

    //        if (!PropertyMappingRegistry.HasPropertyMapping(targetProperty.DeclaringType,
    //            targetProperty.Name, targetProperty.PropertyType))
    //            return false;

    //        return true;
    //    }

    //    private static PropertyInfo ExtractProperty(IMetaExpression child)
    //    {
    //        var memberExpression = (MemberExpression)child.TargetExpression;
    //        var targetMember = memberExpression.Member;

    //        var targetProperty = targetMember as PropertyInfo;
    //        return targetProperty;
    //    }
    //    protected void Visit(StringBuilder builder, IMetaExpression expression)
    //    {
    //        var leftChild = expression.Children[0];
    //        var rightChild = expression.Children[1];
    //        var expressionType = expression.TargetExpression.NodeType;

    //        // Only process registered operator types
    //        if (!_expressionMap.ContainsKey(expressionType))
    //            return;

    //        // Surround the expression in parentheses
    //        StringBuilder currentBuilder = new StringBuilder();

    //        var targetProperty = ExtractProperty(leftChild);
    //        if (targetProperty == null)
    //            return;

    //        var declaringType = targetProperty.DeclaringType;
    //        var propertyName = targetProperty.Name;
    //        var propertyType = targetProperty.PropertyType;

    //        IPropertyMapping mapping = PropertyMappingRegistry.GetPropertyMapping(declaringType, propertyName, propertyType);
    //        if (mapping == null)
    //            return;

    //        // Get the field name associated with the target property
    //        string fieldName = mapping.ColumnName;

    //        // Get the operator
    //        string operatorText = _expressionMap[expressionType];

    //        var constantExpression = rightChild.TargetExpression as ConstantExpression;
    //        if (constantExpression == null)
    //            return;

    //        // Wrap the constant in quotes, if necessary
    //        var value = constantExpression.Value;

    //        if (value != null && value.GetType() == typeof(string))
    //            value = string.Format("'{0}'", value);

    //        builder.AppendFormat("{0} {1} {2}", fieldName, operatorText, value);
    //    }
    //}
}
