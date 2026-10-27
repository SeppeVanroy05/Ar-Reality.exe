class RepairRecord {
  final String glitchId;
  final DateTime repairedAt;
  final int durationSeconds;

  const RepairRecord({
    required this.glitchId,
    required this.repairedAt,
    required this.durationSeconds,
  });

  Map<String, Object?> toMap() => {
    'glitch_id': glitchId,
    'repaired_at': repairedAt.toUtc().toIso8601String(),
    'duration_seconds': durationSeconds,
  };

  factory RepairRecord.fromMap(Map<String, dynamic> map) => RepairRecord(
    glitchId: map['glitch_id'] as String,
    repairedAt: DateTime.parse(map['repaired_at'] as String),
    durationSeconds: map['duration_seconds'] as int,
  );
}
