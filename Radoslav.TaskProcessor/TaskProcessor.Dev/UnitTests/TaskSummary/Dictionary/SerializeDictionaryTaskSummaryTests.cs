using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class SerializeDictionaryTaskSummaryTests<TSerializationData> : TaskSummaryTestsBase<DictionaryTaskSummary, TSerializationData>
    {
        protected override DictionaryTaskSummary CreateTaskSummary(bool empty)
        {
            DictionaryTaskSummary result = new DictionaryTaskSummary();

            if (!empty)
            {
                result.Add("Key 1", "Value 1");
                result.Add("Key 2", "Value 2");
                result.Add("Key 3", "^&*()&*($&#*$}_ IA)S_R(&$#)Q&$ Q*QA()*AU() &)&# ");
            }

            return result;
        }

        protected override void AssertIsEmpty(DictionaryTaskSummary summary)
        {
            if (summary == null)
            {
                throw new ArgumentNullException(nameof(summary));
            }

            Assert.AreEqual(0, summary.Count);
        }

        protected override void AssertAreEqual(DictionaryTaskSummary first, DictionaryTaskSummary second)
        {
            if (first == null)
            {
                if (second == null)
                {
                    return;
                }
                else
                {
                    throw new ArgumentNullException(nameof(first));
                }
            }
            else
            {
                if (second == null)
                {
                    throw new ArgumentNullException(nameof(second));
                }
                else
                {
                    Assert.AreEqual(first.Count, second.Count);

                    foreach (string key in first.Keys)
                    {
                        Assert.AreEqual(first[key], second[key]);
                    }
                }
            }
        }
    }
}