using Daany.MathStuff.Stats;
using Xunit;
using System;

namespace Daany.MathStuff.Tests
{
	public class ConfusionMatrixTests
	{
		#region Test Data
		// Binary classification test data
		private readonly int[] _binaryObserved = { 0, 1, 0, 1, 1, 0, 0, 1, 1, 1 };
		private readonly int[] _binaryPredicted = { 0, 1, 0, 0, 1, 0, 1, 1, 1, 0 };
		private readonly int[][] _binaryMatrix = new int[][]
		{
			new int[] { 3, 1 }, // TN, FP Actual 0: 3 correct, 1 predicted as 1
            new int[] { 2, 4 }, // FN, TP Actual 1: 2 predicted as 0, 4 correct
        };

		// Multi-class (3 classes) test data
		private readonly int[] _multiObserved =  { 0, 1, 2, 0, 1, 2, 0, 1, 2, 0 };
		private readonly int[] _multiPredicted = { 0, 1, 2, 0, 0, 2, 1, 1, 2, 0 };
		private readonly int[][] _multiMatrix = new int[][]
		{
			new int[] { 3, 1, 0 }, // Actual 0: 2 correct, 1 predicted as 1
            new int[] { 1, 2, 0 }, // Actual 1: 1 predicted as 0, 2 correct
            new int[] { 0, 0, 3 }  // Actual 2: all 3 correct
        };

		// Perfect classification test data
		private readonly int[] _perfectObserved = { 0, 1, 0, 1 };
		private readonly int[] _perfectPredicted = { 0, 1, 0, 1 };
		private readonly int[][] _perfectMatrix = new int[][]
		{
			new int[] { 2, 0 },
			new int[] { 0, 2 }
		}; 

		// Worst classification test data
		private readonly int[] _worstObserved = { 0, 1, 0, 1 };
		private readonly int[] _worstPredicted = { 1, 0, 1, 0 };
		private readonly int[][] _worstMatrix = new int[][]
		{
			new int[] { 0, 2 },
			new int[] { 2, 0 }
		};

		// Single class test data
		private readonly int[] _singleObserved = { 0, 0, 0, 0 };
		private readonly int[] _singlePredicted = { 0, 0, 0, 0 };
		private readonly int[][] _singleMatrix = new int[][]
		{
			new int[] { 4 }
		};
		#endregion

		#region Constructor Tests
		[Fact]
		public void Constructor_WithBinaryData_CreatesCorrectMatrix()
		{
			var cm = new ConfusionMatrix(_binaryObserved, _binaryPredicted, 2);

			Assert.Equal(2, cm.ClassCount);
			Assert.Equal(10, cm.TotalSamples);
			Assert.True(cm.IsBinary);
			Assert.Equal(_binaryMatrix[0], cm.Matrix[0]);
			Assert.Equal(_binaryMatrix[1], cm.Matrix[1]);
		}

		[Fact]
		public void Constructor_WithMultiClassData_CreatesCorrectMatrix()
		{
			var cm = new ConfusionMatrix(_multiObserved, _multiPredicted, 3);

			Assert.Equal(3, cm.ClassCount);
			Assert.Equal(10, cm.TotalSamples);
			Assert.False(cm.IsBinary);
			Assert.Equal(_multiMatrix[0], cm.Matrix[0]);
			Assert.Equal(_multiMatrix[1], cm.Matrix[1]);
			Assert.Equal(_multiMatrix[2], cm.Matrix[2]);
		}

		[Fact]
		public void Constructor_WithPrecomputedMatrix_CreatesCorrectMatrix()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);

			Assert.Equal(2, cm.ClassCount);
			Assert.Equal(10, cm.TotalSamples);
			Assert.Equal(_binaryMatrix[0], cm.Matrix[0]);
			Assert.Equal(_binaryMatrix[1], cm.Matrix[1]);
		}

		[Fact]
		public void Constructor_WithInvalidData_ThrowsException()
		{
			var invalidObserved = new int[] { 0, 1, 2 };
			var invalidPredicted = new int[] { 0, 1 };

			Assert.Throws<ArgumentException>(() => new ConfusionMatrix(invalidObserved, invalidPredicted, 2));
		}

		[Fact]
		public void Constructor_WithInvalidMatrix_ThrowsException()
		{
			var invalidMatrix = new int[][]
			{
				new int[] { 1, 2 },
				new int[] { 3 } // Not square
            };

			Assert.Throws<ArgumentException>(() => new ConfusionMatrix(invalidMatrix));
		}
		#endregion

		#region Basic Metrics Tests
		[Fact]
		public void TruePositives_ForBinaryClass_ReturnsCorrectCount()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);

			Assert.Equal(3, cm.TN); // Class 0
			Assert.Equal(4, cm.TP); // Class 1
		}

		[Fact]
		public void FalsePositives_ForBinaryClass_ReturnsCorrectCount()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);

			Assert.Equal(1, cm.FP); 
			Assert.Equal(2, cm.FN); 
		}

		[Fact]
		public void ClassWeights_CalculatesCorrectWeights()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);

			Assert.Equal(0.4m, cm.GetClassWeight(0)); // 4/10 samples are class 0
			Assert.Equal(0.6m, cm.GetClassWeight(1)); // 6/10 samples are class 1
		}
		#endregion

		#region Accuracy Tests
		[Fact]
		public void OverallAccuracy_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// (3 + 4) correct out of 10
			Assert.Equal(0.7m, cm.OverallAccuracy);
		}

		[Fact]
		public void AverageAccuracy_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// Class 0: (3 + 4)/10 = 0.7
			// Class 1: (4 + 3)/10 = 0.7
			// Average = 0.7
			Assert.Equal(0.7m, cm.AverageAccuracy, 2);
		}

		[Fact]
		public void BalancedAccuracy_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// Class 0 recall: 3/(3+1) = 0.75
			// Class 1 recall: 4/(4+2) ≈ 0.6667
			// Average = (0.75 + 0.6667)/2 ≈ 0.7083
			Assert.Equal(0.7083m, cm.BalancedAccuracy, 3);
		}

		[Fact]
		public void ErrorRate_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// 1 - overall accuracy (0.7)
			Assert.Equal(0.3m, cm.ErrorRate);
		}

		[Fact]
		public void AverageErrorRate_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// 1 - average accuracy (0.7)
			Assert.Equal(0.3m, cm.AverageErrorRate, 2);
		}

		[Fact]
		public void ClassAccuracy_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// Class 0: (3 TP + 4 TN)/10 = 0.7
			Assert.Equal(0.7m, cm.ClassAccuracy(0));
			// Class 1: (4 TP + 3 TN)/10 = 0.7
			Assert.Equal(0.7m, cm.ClassAccuracy(1));
		}
		#endregion

		#region Precision/Recall Tests
		[Fact]
		public void Precision_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// Class 0: 3 TP / (3 TP + 2 FP) = 0.6
			Assert.Equal(0.6m, cm.Precision(0));
			// Class 1: 4 TP / (4 TP + 1 FP) = 0.8
			Assert.Equal(0.8m, cm.Precision(1));
		}

		[Fact]
		public void Recall_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// Class 0: 3 TP / (3 TP + 1 FN) = 0.75
			Assert.Equal(0.75m, cm.Recall(0));
			// Class 1: 4 TP / (4 TP + 2 FN) ≈ 0.6667
			Assert.Equal(0.6667m, cm.Recall(1), 3);
		}

		[Fact]
		public void MicroPrecision_EqualsMicroRecall_EqualsAccuracy()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			var microPrecision = cm.MicroPrecision;
			var microRecall = cm.MicroRecall;
			var accuracy = cm.OverallAccuracy;

			Assert.Equal(accuracy, microPrecision);
			Assert.Equal(accuracy, microRecall);
		}

		[Fact]
		public void MacroPrecision_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// (0.6 + 0.8)/2 = 0.7
			Assert.Equal(0.7m, cm.MacroPrecision);
		}

		[Fact]
		public void MacroRecall_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// (0.75 + 0.6667)/2 ≈ 0.7083
			Assert.Equal(0.7083m, cm.MacroRecall, 3);
		}

		[Fact]
		public void WeightedPrecision_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// Class 0 weight: 0.4, precision: 0.6 → 0.24
			// Class 1 weight: 0.6, precision: 0.8 → 0.48
			// Sum: 0.72
			Assert.Equal(0.72m, cm.WeightedPrecision);
		}

		[Fact]
		public void WeightedRecall_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// Class 0 weight: 0.4, recall: 0.75 → 0.3
			// Class 1 weight: 0.6, recall: 0.6667 → 0.4
			// Sum: 0.7
			Assert.Equal(0.7m, cm.WeightedRecall, 3);
		}
		#endregion

		#region F-Score Tests
		[Fact]
		public void F1Score_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// Class 0: 2*(0.6*0.75)/(0.6+0.75) ≈ 0.6667
			Assert.Equal(0.6667m, cm.F1Score(0), 3);
			// Class 1: 2*(0.8*0.6667)/(0.8+0.6667) ≈ 0.7273
			Assert.Equal(0.7273m, cm.F1Score(1), 3);
		}

		[Fact]
		public void FBetaScore_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// Class 0 with beta=2: 5*(0.6*0.75)/(4*0.6 + 0.75) ≈ 0.6944
			Assert.Equal(0.71428m, cm.FBetaScore(0, 2), 3);
			// Class 1 with beta=0.5: 1.25*(0.8*0.6667)/(0.25*0.8 + 0.6667) ≈ 0.7407
			Assert.Equal(0.7692m, cm.FBetaScore(1, 0.5m), 3);
		}

		[Fact]
		public void MicroF1Score_EqualsAccuracy()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			Assert.Equal(cm.OverallAccuracy, cm.MicroF1Score);
		}

		[Fact]
		public void MacroF1Score_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// (0.6667 + 0.7273)/2 ≈ 0.6970
			Assert.Equal(0.6970m, cm.MacroF1Score, 3);
		}

		[Fact]
		public void WeightedF1Score_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// Class 0 weight: 0.4, F1: 0.6667 → 0.2667
			// Class 1 weight: 0.6, F1: 0.7273 → 0.4364
			// Sum: ≈ 0.7030
			Assert.Equal(0.7030m, cm.WeightedF1Score, 3);
		}
		#endregion

		#region Specificity Tests
		[Fact]
		public void Specificity_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// Class 0: 4 TN / (4 TN + 2 FP) ≈ 0.6667
			Assert.Equal(0.6667m, cm.Specificity(0), 3);
			// Class 1: 3 TN / (3 TN + 1 FP) = 0.75
			Assert.Equal(0.75m, cm.Specificity(1));
		}

		[Fact]
		public void TrueNegativeRate_EqualsSpecificity()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			Assert.Equal(cm.Specificity(0), cm.TrueNegativeRate(0));
			Assert.Equal(cm.Specificity(1), cm.TrueNegativeRate(1));
		}
		#endregion

		#region Skill Scores Tests
		[Fact]
		public void MatthewsCorrelationCoefficient_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// (3*4 - 2*1)/sqrt((3+2)(3+1)(4+2)(4+1)) ≈ 0.408
			Assert.Equal(0.4082m, cm.MatthewsCorrelationCoefficient, 3);
		}

		[Fact]
		public void MatthewsCorrelationCoefficient_ForNonBinary_ThrowsException()
		{
			var cm = new ConfusionMatrix(_multiMatrix);
			Assert.Throws<InvalidOperationException>(() => cm.MatthewsCorrelationCoefficient);
		}

		[Fact]
		public void CohensKappa_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// po = 0.7
			// pe = (4*5 + 6*5)/100 = 0.5
			// kappa = (0.7-0.5)/(1-0.5) = 0.4
			Assert.Equal(0.4m, cm.CohensKappa);
		}

		[Fact]
		public void HeidkeSkillScore_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// nc = 7, e = (4*5 + 6*5)/10 = 5
			// hss = (7-5)/(10-5) = 0.4
			Assert.Equal(0.4m, cm.HeidkeSkillScore);
		}

		[Fact]
		public void PierceSkillScore_CalculatesCorrectValue()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);
			// nc = 7, e = 5, ep = (4*4 + 6*6)/10 = 5.2
			// pss = (7-5)/(10-5.2) ≈ 0.4167
			Assert.Equal(0.4167m, cm.PierceSkillScore, 3);
		}
		#endregion

		#region Edge Cases Tests
		[Fact]
		public void PerfectClassification_HasPerfectMetrics()
		{
			var cm = new ConfusionMatrix(_perfectMatrix);

			Assert.Equal(1.0m, cm.OverallAccuracy);
			Assert.Equal(1.0m, cm.MacroPrecision);
			Assert.Equal(1.0m, cm.MacroRecall);
			Assert.Equal(1.0m, cm.MacroF1Score);
			Assert.Equal(1.0m, cm.CohensKappa);
			Assert.Equal(1.0m, cm.MatthewsCorrelationCoefficient);
		}

		[Fact]
		public void WorstClassification_HasWorstMetrics()
		{
			var cm = new ConfusionMatrix(_worstMatrix);

			Assert.Equal(0.0m, cm.OverallAccuracy);
			Assert.Equal(0.0m, cm.MacroPrecision);
			Assert.Equal(0.0m, cm.MacroRecall);
			Assert.Equal(0.0m, cm.MacroF1Score);
			Assert.Equal(-1.0m, cm.CohensKappa);
			Assert.Equal(-1.0m, cm.MatthewsCorrelationCoefficient);
		}

		[Fact]
		public void SingleClass_HasPerfectMetrics()
		{
			var cm = new ConfusionMatrix(_singleMatrix);

			Assert.Equal(1.0m, cm.OverallAccuracy);
			Assert.Equal(1.0m, cm.MacroPrecision);
			Assert.Equal(1.0m, cm.MacroRecall);
			Assert.Equal(1.0m, cm.MacroF1Score);
		}

		[Fact]
		public void EmptyMatrix_ReturnsZeroMetrics()
		{
			var emptyMatrix = new int[][] { new int[] { 0, 0 }, new int[] { 0, 0 } };
			var cm = new ConfusionMatrix(emptyMatrix);

			Assert.Equal(0.0m, cm.OverallAccuracy);
			Assert.Equal(0.0m, cm.MacroPrecision);
			Assert.Equal(0.0m, cm.MacroRecall);
			Assert.Equal(0.0m, cm.MacroF1Score);
			Assert.Equal(0.0m, cm.CohensKappa);
		}
		#endregion

		#region Binary Shortcut Tests
		[Fact]
		public void BinaryShortcuts_ReturnCorrectValues()
		{
			var cm = new ConfusionMatrix(_binaryMatrix);

			Assert.Equal(4, cm.TP);
			Assert.Equal(3, cm.TN);
			Assert.Equal(1, cm.FP);
			Assert.Equal(2, cm.FN);

			Assert.Equal(0.8m, cm.BinaryPrecision);
			Assert.Equal(0.6667m, cm.BinaryRecall, 3);
			Assert.Equal(0.7273m, cm.BinaryF1Score, 3);
			Assert.Equal(0.75m, cm.BinarySpecificity);
		}

		[Fact]
		public void BinaryShortcuts_ForNonBinary_ThrowException()
		{
			var cm = new ConfusionMatrix(_multiMatrix);
			Assert.Throws<InvalidOperationException>(() => cm.TP);
		}
		#endregion
	}
}