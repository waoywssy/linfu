using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Linq.TreeMasher;
using System.Data.SqlClient;

namespace LinFu.Persist
{
    //// Disclaimer: This provider is *far* from production quality.
    //// Use it at your own risk!
    //public class WhereQueryProvider : IQueryInterpreter
    //{
    //    public object Execute(Expression expression)
    //    {
    //        List<IMetaExpression> results = TreeMasher.Mash(expression);
    //        results.Sort(SortBySequence);


    //        // TODO: Validate the structure of this query

    //        // Only 'where' queries are supported
    //        var rootQuery = from m in results
    //                        where m.Parent == null && m.Depth == 1 &&
    //                        m.TargetExpression is MethodCallExpression &&
    //                        m.TargetExpression.As<MethodCallExpression>().Method.Name == "Where"
    //                        select m;



    //        var queryResults = rootQuery.ToList();

    //        if (queryResults.Count == 0)
    //            throw new NotSupportedException("Only 'where' queries are supported");

    //        IMetaExpression root = queryResults.First();



    //        // Determine the parameter type
    //        var parameterExpressionList = (from e in results
    //                                       where e.Depth == 4 &&
    //                                       e.ExpressionType == typeof(ParameterExpression)
    //                                       select e).ToList();


    //        if (parameterExpressionList.Count == 0)
    //            throw new NotSupportedException("This query type is not supported");


    //        var parameterExpression = parameterExpressionList[0].TargetExpression.As<ParameterExpression>();
    //        Type targetType = parameterExpression.Type;



    //        // Get the subtree that holds the criteria for the 'where' clause
    //        var criteriaTree = (from e in results
    //                            where e.Depth == 4 &&
    //                            e.ExpressionType == typeof(BinaryExpression)
    //                            select e).ToList();

    //        // If the where criteria is empty, then this query is invalid
    //        if (parameterExpressionList.Count == 0)
    //            throw new NotSupportedException("Invalid expression tree");

    //        var subTree = criteriaTree.First();

    //        // Determine the name of the table
    //        string tableName = GetTableName(targetType);

    //        StringBuilder stringBuilder = new StringBuilder();
    //        WhereClauseBuilder clauseBuilder = new WhereClauseBuilder(stringBuilder);
    //        clauseBuilder.Visit(subTree);

    //        string whereClause = stringBuilder.ToString();


    //        // TODO: Add support for parameterized SQL queries
    //        string queryText = string.Format("select * from {0} where {1}", tableName, whereClause);

    //        // UGLY, UGLY HACK: Insert your connection string here
    //        string connectionString = @"Data Source=(local);Initial Catalog=Northwind;Integrated Security=True";

    //        SqlConnection connection = new SqlConnection(connectionString);
    //        connection.Open();

    //        var schemaText = string.Format("select * from {0} where 1!=1", tableName);
    //        SqlCommand schemaCommand = new SqlCommand(schemaText, connection);


    //        // TODO: Make the database code provider-independent
    //        SqlCommand command = new SqlCommand(queryText, connection);

    //        SqlTable table = new SqlTable();

    //        // TODO: Insert an injection call to Simple.IOC at this point
    //        var mapperRegistry = new MapperRegistry();

    //        table.PrimaryKeyField = "OrderID";
    //        table.TableName = "Orders";
    //        table.RowRegistry = new RowRegistry();

    //        table.CreateSchemaFrom(schemaCommand);
    //        table.FillWith(command);

    //        //// HACK: Manually construct the mapper for the orders class

    //        //var strategy = new DefaultMapper();
    //        //var compositeBehavior = new CompositePropertyAssignmentBehavior();
    //        //strategy.PropertyAssignmentBehavior = compositeBehavior;
    //        //var orderInstantiator = new InterfaceInstantiator();
    //        //orderInstantiator.AddImplementation(typeof(Order), typeof(Order));
    //        //strategy.Instantiator = orderInstantiator;
    //        //mapperRegistry.Register(strategy, typeof(Order), table.Columns);

    //        //// TODO: Generalize the mapper construction process
    //        //if (!mapperRegistry.HasMapperFor(typeof(Order), table.Columns))
    //        //    throw new NotSupportedException();

    //        //IList<Order> matches = new List<Order>();
    //        //foreach (var row in table.Rows)
    //        //{
    //        //    var item = (Order)strategy.CreateItem(typeof(Order), row);
    //        //    if (item == null)
    //        //        continue;

    //        //    matches.Add(item);
    //        //}

    //        //connection.Close();
    //        //return matches;
    //        throw new NotImplementedException("Manually uncomment this block of code for testing");
    //    }

       
    //    // TODO: Convert this to an interface method call
    //    protected string GetTableName(Type targetType)
    //    {
    //        //if (targetType == typeof(Order))
    //        //    return "Orders";

    //        throw new NotSupportedException();
    //    }
    //    private static int SortBySequence(IMetaExpression first, IMetaExpression second)
    //    {
    //        if (first.Sequence == second.Sequence)
    //            return 0;

    //        if (first.Sequence < second.Sequence)
    //            return -1;

    //        return 1;
    //    }
    //}
}
