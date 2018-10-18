using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.TaskProcessor.Model;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public abstract class SerializeDictionaryTaskJobSettingsTests<TSerializationData> : TaskJobSettingsTestsBase<DictionaryTaskJobSettings, TSerializationData>
    {
        protected override DictionaryTaskJobSettings CreateTaskJobSettings(bool empty)
        {
            DictionaryTaskJobSettings result = new DictionaryTaskJobSettings();

            if (!empty)
            {
                result.Add("Key 1", "Value 1");
                result.Add("Key 2", "Value 2");
                result.Add("Key 3", "^&*()&*($&#*$}_ IA)S_R(&$#)Q&$ Q*QA()*AU() &)&# ");
            }

            return result;
        }

        protected override void AssertIsEmpty(DictionaryTaskJobSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Assert.AreEqual(0, settings.Count);
        }

        protected override void AssertAreEqual(DictionaryTaskJobSettings first, DictionaryTaskJobSettings second)
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