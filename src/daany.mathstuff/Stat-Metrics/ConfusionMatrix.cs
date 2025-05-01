using System;
using System.Linq;

namespace Daany.MathStuff.Stats;

/// <summary>
/// Implementation of Confusion Matrix for binary and multi-class classification evaluation.
/// Provides comprehensive performance metrics including accuracy, precision, recall, F-scores, and skill scores.
/// </summary>
public class ConfusionMatrix
{
	#region Properties and Fields
	private readonly int[][] _matrix;
	private readonly int _totalSamples;
	private readonly Lazy<decimal[]> _classWeights;

	/// <summary>
	/// The confusion matrix data
	/// </summary>
	public int[][] Matrix => _matrix;

	/// <summary>
	/// Number of classes in the confusion matrix
	/// </summary>
	public int ClassCount => _matrix.Length;

	/// <summary>
	/// Total number of samples used to create the matrix
	/// </summary>
	public int TotalSamples => _totalSamples;

	/// <summary>
	/// Indicates if this is a binary classification matrix
	/// </summary>
	public bool IsBinary => ClassCount == 2;

	/// <summary>
	/// Indicates if the matrix was created from sampled data
	/// </summary>
	public bool IsSampled { get; }

	/// <summary>
	/// Indicates if the matrix uses weighted calculations
	/// </summary>
	public bool IsWeighted { get; }
	#endregion

	#region Constructors
	/// <summary>
	/// Creates a confusion matrix from pre-calculated values
	/// </summary>
	public ConfusionMatrix(int[][] matrix, bool isSampled = false, bool isWeighted = false)
	{
		if (matrix == null || matrix.Length == 0 || matrix.Any(row => row == null || row.Length != matrix.Length))
		{
			throw new ArgumentException("Confusion matrix must be a non-null square matrix");
		}

		_matrix = matrix;
		IsSampled = isSampled;
		IsWeighted = isWeighted;
		_totalSamples = matrix.Sum(row => row.Sum());
		_classWeights = new Lazy<decimal[]>(CalculateClassWeights);
	}

	/// <summary>
	/// Creates a confusion matrix from observed and predicted values
	/// </summary>
	public ConfusionMatrix(int[] observed, int[] predicted, int classCount, bool isSampled = false, bool isWeighted = false)
	{
		if (observed == null || predicted == null || observed.Length == 0 || observed.Length != predicted.Length)
		{
			throw new ArgumentException("Observed and predicted arrays must be non-null and of equal length");
		}

		if (classCount <= 0)
		{
			throw new ArgumentException("Class count must be positive");
		}

		_matrix = new int[classCount][];
		for (int i = 0; i < classCount; i++)
		{
			_matrix[i] = new int[classCount];
		}

		_totalSamples = observed.Length;
		IsSampled = isSampled;
		IsWeighted = isWeighted;

		for (int i = 0; i < observed.Length; i++)
		{
			int o = observed[i];
			int p = predicted[i];

			if (o < 0 || o >= classCount || p < 0 || p >= classCount)
			{
				throw new ArgumentException($"Class indices must be between 0 and {classCount - 1}");
			}

			_matrix[o][p]++;
		}

		_classWeights = new Lazy<decimal[]>(CalculateClassWeights);
	}
	#endregion

	#region Core Metrics Calculation
	private decimal[] CalculateClassWeights()
	{
		decimal[] weights = new decimal[ClassCount];
		int total = TotalSamples;

		if (total == 0) return weights;

		for (int i = 0; i < ClassCount; i++)
		{
			weights[i] = _matrix[i].Sum() / (decimal)total;
		}

		return weights;
	}

	public decimal GetClassWeight(int classIndex)
	{
		if (classIndex < 0 || classIndex >= ClassCount)
		{
			throw new ArgumentOutOfRangeException(nameof(classIndex), $"Class index must be between 0 and {ClassCount - 1}");
		}
		return _classWeights.Value[classIndex];
	}

	public int GetTruePositives(int classIndex) => _matrix[classIndex][classIndex];

	public int GetFalseNegatives(int classIndex) => _matrix[classIndex].Sum() - GetTruePositives(classIndex);

	public int GetFalsePositives(int classIndex)
	{
		int fp = 0;
		for (int i = 0; i < ClassCount; i++)
			if (i != classIndex) fp += _matrix[i][classIndex];
		return fp;
	}

	public int GetTrueNegatives(int classIndex)
	{
		int tn = 0;
		for (int i = 0; i < ClassCount; i++)
			if (i != classIndex)
				for (int j = 0; j < ClassCount; j++)
					if (j != classIndex) tn += _matrix[i][j];
		return tn;
	}
	#endregion

	#region Accuracy Metrics
	public decimal OverallAccuracy
	{
		get
		{
			if (TotalSamples == 0) return 0;
			int correct = 0;
			for (int i = 0; i < ClassCount; i++)
				correct += _matrix[i][i];
			return correct / (decimal)TotalSamples;
		}
	}

	public decimal AverageAccuracy
	{
		get
		{
			decimal sum = 0;
			for (int i = 0; i < ClassCount; i++)
				sum += ClassAccuracy(i);
			return sum / ClassCount;
		}
	}

	public decimal ClassAccuracy(int classIndex)
	{
		int tp = GetTruePositives(classIndex);
		int tn = GetTrueNegatives(classIndex);
		int fp = GetFalsePositives(classIndex);
		int fn = GetFalseNegatives(classIndex);

		decimal denominator = tp + tn + fp + fn;
		return denominator == 0 ? 0 : (tp + tn) / denominator;
	}

	public decimal BalancedAccuracy
	{
		get
		{
			decimal sum = 0;
			for (int i = 0; i < ClassCount; i++)
				sum += Recall(i);
			return sum / ClassCount;
		}
	}

	public decimal ErrorRate => 1 - OverallAccuracy;

	public decimal AverageErrorRate => 1 - AverageAccuracy;
	#endregion

	#region Precision/Recall Metrics
	public decimal MicroPrecision
	{
		get
		{
			decimal tp = 0, fp = 0;
			for (int i = 0; i < ClassCount; i++)
			{
				tp += GetTruePositives(i);
				fp += GetFalsePositives(i);
			}
			decimal denominator = tp + fp;
			return denominator == 0 ? 0 : tp / denominator;
		}
	}

	public decimal MacroPrecision
	{
		get
		{
			decimal sum = 0;
			for (int i = 0; i < ClassCount; i++)
				sum += Precision(i);
			return sum / ClassCount;
		}
	}

	public decimal WeightedPrecision
	{
		get
		{
			decimal sum = 0;
			var weights = _classWeights.Value;
			for (int i = 0; i < ClassCount; i++)
				sum += Precision(i) * weights[i];
			return sum;
		}
	}

	public decimal Precision(int classIndex)
	{
		int tp = GetTruePositives(classIndex);
		int fp = GetFalsePositives(classIndex);
		decimal denominator = tp + fp;
		return denominator == 0 ? 0 : tp / denominator;
	}

	public decimal MicroRecall
	{
		get
		{
			decimal tp = 0, fn = 0;
			for (int i = 0; i < ClassCount; i++)
			{
				tp += GetTruePositives(i);
				fn += GetFalseNegatives(i);
			}
			decimal denominator = tp + fn;
			return denominator == 0 ? 0 : tp / denominator;
		}
	}

	public decimal MacroRecall
	{
		get
		{
			decimal sum = 0;
			for (int i = 0; i < ClassCount; i++)
				sum += Recall(i);
			return sum / ClassCount;
		}
	}

	public decimal WeightedRecall
	{
		get
		{
			decimal sum = 0;
			var weights = _classWeights.Value;
			for (int i = 0; i < ClassCount; i++)
				sum += Recall(i) * weights[i];
			return sum;
		}
	}

	public decimal Recall(int classIndex)
	{
		int tp = GetTruePositives(classIndex);
		int fn = GetFalseNegatives(classIndex);
		decimal denominator = tp + fn;
		return denominator == 0 ? 0 : tp / denominator;
	}

	public decimal Sensitivity(int classIndex) => Recall(classIndex);
	public decimal TruePositiveRate(int classIndex) => Recall(classIndex);
	#endregion

	#region Specificity Metrics
	public decimal Specificity(int classIndex)
	{
		int tn = GetTrueNegatives(classIndex);
		int fp = GetFalsePositives(classIndex);
		decimal denominator = tn + fp;
		return denominator == 0 ? 0 : tn / denominator;
	}

	public decimal TrueNegativeRate(int classIndex) => Specificity(classIndex);
	#endregion

	#region F-Score Metrics
	public decimal MicroF1Score
	{
		get
		{
			decimal microPrecision = MicroPrecision;
			decimal microRecall = MicroRecall;
			decimal denominator = microPrecision + microRecall;
			return denominator == 0 ? 0 : 2 * microPrecision * microRecall / denominator;
		}
	}

	public decimal MacroF1Score
	{
		get
		{
			decimal sum = 0;
			for (int i = 0; i < ClassCount; i++)
				sum += F1Score(i);
			return sum / ClassCount;
		}
	}

	public decimal WeightedF1Score
	{
		get
		{
			decimal sum = 0;
			var weights = _classWeights.Value;
			for (int i = 0; i < ClassCount; i++)
				sum += F1Score(i) * weights[i];
			return sum;
		}
	}

	public decimal F1Score(int classIndex)
	{
		decimal p = Precision(classIndex);
		decimal r = Recall(classIndex);
		decimal denominator = p + r;
		return denominator == 0 ? 0 : 2 * p * r / denominator;
	}

	public decimal FBetaScore(int classIndex, decimal beta)
	{
		decimal p = Precision(classIndex);
		decimal r = Recall(classIndex);
		decimal betaSquared = beta * beta;
		decimal denominator = betaSquared * p + r;
		return denominator == 0 ? 0 : (1 + betaSquared) * p * r / denominator;
	}
	#endregion

	#region Skill Scores
	public decimal MatthewsCorrelationCoefficient
	{
		get
		{
			if (!IsBinary)
				throw new InvalidOperationException("MCC is only defined for binary classification");

			int tp = GetTruePositives(1);
			int tn = GetTruePositives(0);
			int fp = GetFalsePositives(1);
			int fn = GetFalseNegatives(1);

			decimal numerator = tp * tn - fp * fn;
			decimal denominator = (decimal)Math.Sqrt((double)((tp + fp) * (tp + fn) * (tn + fp) * (tn + fn)));
			return denominator == 0 ? 0 : numerator / denominator;
		}
	}

	public decimal CohensKappa
	{
		get
		{
			if (TotalSamples == 0) return 0;

			decimal po = OverallAccuracy;
			decimal pe = 0;

			for (int i = 0; i < ClassCount; i++)
			{
				decimal rowSum = _matrix[i].Sum();
				decimal colSum = 0;
				for (int j = 0; j < ClassCount; j++)
					colSum += _matrix[j][i];
				pe += rowSum * colSum;
			}

			pe /= TotalSamples * TotalSamples;
			return (po - pe) / (1 - pe);
		}
	}
	public decimal HeidkeSkillScore
	{
		get
		{
			if (TotalSamples == 0) return 0;

			decimal nc = 0; // number of correct predictions
			decimal e = 0;  // expected correct predictions by chance

			decimal[] rowSums = new decimal[ClassCount];
			decimal[] colSums = new decimal[ClassCount];

			for (int i = 0; i < ClassCount; i++)
			{
				nc += _matrix[i][i];
				for (int j = 0; j < ClassCount; j++)
				{
					rowSums[i] += _matrix[i][j];
					colSums[j] += _matrix[i][j];
				}
			}

			for (int i = 0; i < ClassCount; i++)
			{
				e += rowSums[i] * colSums[i];
			}

			e /= TotalSamples;

			decimal denominator = TotalSamples - e;
			return denominator == 0 ? 0 : (nc - e) / denominator;
		}
	}

	public decimal PierceSkillScore
	{
		get
		{
			if (TotalSamples == 0) return 0;

			decimal nc = 0; // number of correct predictions
			decimal e = 0;  // expected correct predictions by chance
			decimal ep = 0; // expected correct predictions by always predicting the most frequent class

			decimal[] rowSums = new decimal[ClassCount];
			decimal[] colSums = new decimal[ClassCount];

			for (int i = 0; i < ClassCount; i++)
			{
				nc += _matrix[i][i];
				for (int j = 0; j < ClassCount; j++)
				{
					rowSums[i] += _matrix[i][j];
					colSums[j] += _matrix[i][j];
				}
			}

			for (int i = 0; i < ClassCount; i++)
			{
				e += rowSums[i] * colSums[i];
				ep += rowSums[i] * rowSums[i];
			}

			e /= TotalSamples;
			ep /= TotalSamples;

			decimal denominator = TotalSamples - ep;
			return denominator == 0 ? 0 : (nc - e) / denominator;
		}
	}
	#endregion

	#region Additional Metrics
	public decimal Prevalence(int classIndex)
	{
		if (TotalSamples == 0) return 0;
		return _matrix[classIndex].Sum() / (decimal)TotalSamples;
	}

	public decimal PositivePredictiveValue(int classIndex) => Precision(classIndex);

	public decimal NegativePredictiveValue(int classIndex)
	{
		int tn = GetTrueNegatives(classIndex);
		int fn = GetFalseNegatives(classIndex);
		decimal denominator = tn + fn;
		return denominator == 0 ? 0 : tn / denominator;
	}

	public decimal FalsePositiveRate(int classIndex) => 1 - Specificity(classIndex);

	public decimal FalseDiscoveryRate(int classIndex) => 1 - Precision(classIndex);

	public decimal FalseOmissionRate(int classIndex) => 1 - NegativePredictiveValue(classIndex);

	public decimal ThreatScore(int classIndex)
	{
		int tp = GetTruePositives(classIndex);
		int fp = GetFalsePositives(classIndex);
		int fn = GetFalseNegatives(classIndex);
		decimal denominator = tp + fp + fn;
		return denominator == 0 ? 0 : tp / denominator;
	}
	#endregion

	#region Binary Classification Shortcuts
	public int TP => IsBinary ? GetTruePositives(1) : throw new InvalidOperationException("Only for binary classification");
	public int TN => IsBinary ? GetTruePositives(0) : throw new InvalidOperationException("Only for binary classification");
	public int FP => IsBinary ? GetFalsePositives(1) : throw new InvalidOperationException("Only for binary classification");
	public int FN => IsBinary ? GetFalseNegatives(1) : throw new InvalidOperationException("Only for binary classification");

	public decimal BinaryPrecision => IsBinary ? Precision(1) : throw new InvalidOperationException("Only for binary classification");
	public decimal BinaryRecall => IsBinary ? Recall(1) : throw new InvalidOperationException("Only for binary classification");
	public decimal BinarySpecificity => IsBinary ? Specificity(1) : throw new InvalidOperationException("Only for binary classification");
	public decimal BinaryF1Score => IsBinary ? F1Score(1) : throw new InvalidOperationException("Only for binary classification");
	#endregion
}