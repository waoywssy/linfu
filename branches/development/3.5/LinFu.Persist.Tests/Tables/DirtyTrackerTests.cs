using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace LinFu.Persist.Tests.Tables
{
    [TestFixture]
    public class DirtyTrackerTests : BaseFixture
    {
        [Test]
        public void TrackedRowShouldReportModifiedCells()
        {
            var newRow = new Row();
            TrackedRow trackedRow = new TrackedRow(newRow);
            trackedRow.TrackingEnabled = false;

            // The tracked row should not mark the new cell
            // as modified
            trackedRow.Cells["OrderID"] = new Cell();
            Assert.IsTrue(trackedRow.ModifiedColumns.Count() == 0);

            trackedRow.TrackingEnabled = true;
            trackedRow.Cells["OrderID"].Value = 12345;

            Assert.IsTrue(trackedRow.ModifiedColumns.Contains("OrderID"));
            Assert.AreEqual(trackedRow.Cells["OrderID"].Value, 12345);
        }

        [Test]
        public void TrackedRowResetShouldReportItselfAsNotDirty()
        {
            var newRow = new Row();
            TrackedRow trackedRow = new TrackedRow(newRow);
            trackedRow.TrackingEnabled = true;

            trackedRow.Cells["OrderID"] = new Cell() { Value = 12345 };
            Assert.IsTrue(trackedRow.ModifiedColumns.Count() > 0);
            trackedRow.Reset();

            Assert.IsTrue(trackedRow.IsDirty == false);
            Assert.IsTrue(trackedRow.ModifiedColumns.Count() == 0);
        }
        [Test]
        public void TableShouldReportAddedRows()
        {
            var newRow = new Row();
            TrackedTable table = new TrackedTable();
            table.Rows.Add(newRow);

            table.Rows[0].Cells["OrderID"] = new Cell();
            table.Rows[0].Cells["OrderID"].Value = 12345;

            Assert.IsTrue(table.AddedRows.Count() > 0);
            Assert.IsTrue(table.ModifiedRows.Count() > 0);

            var modified = table.ModifiedRows.First() as ITrackedRow;
            Assert.IsTrue(modified.ModifiedColumns.Contains("OrderID"));
        }
        [Test]
        public void TableResetShouldReportNoAddedRows()
        {
            var newRow = new Row();
            TrackedTable table = new TrackedTable();
            table.Rows.Add(newRow);

            Assert.IsTrue(table.AddedRows.Count() > 0);
            table.Reset();

            Assert.AreEqual(0, table.AddedRows.Count());

            var tracked = table.Rows[0] as ITrackedRow;
            Assert.IsNotNull(tracked);
            Assert.IsFalse(tracked.IsDirty);
        }
        [Test]
        public void TableShouldReportDeletedRows()
        {
            var newRow = new Row();
            TrackedTable table = new TrackedTable();
            table.Rows.Add(newRow);
            table.Reset();

            table.Rows.Clear();
            Assert.IsTrue(table.DeletedRows.Count() > 0);
            
            var firstRow = table.Rows[0].As<ITrackedRow>();
            Assert.IsNotNull(firstRow);
        }
        [Test]
        public void TableShouldReportModifiedRows()
        {
            var newRow = new Row();
            TrackedTable table = new TrackedTable();
            table.Rows.Add(newRow);
            table.Reset();

            table.Rows[0].Cells["OrderID"] = new Cell() { Value = 12345 };
            Assert.IsTrue(table.ModifiedRows.Count() > 0);

            var firstRow = table.Rows[0].As<ITrackedRow>();
            Assert.IsNotNull(firstRow);
        }
    }
}
