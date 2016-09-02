﻿using Verse;

namespace DefensivePositions {
	/**
	 * Interactions with the control can have different outcomes when multiple pawns are selected. 
	 * This class collects the data and reports on in on the next frame.
	 */
	public class ScheduledReportManager {
		public enum ReportType {
			SavedPosition,
			SentToSavedPosition
		}

		private class ScheduledReport {
			public ReportType reportType;
			public int numPawnsSavedPosition;
			public int numPawnsHadTargetPosition;
			public int numPawnsHadNoTargetPosition;
			public int controlIndex;
			public string noTargetPositionNames;
		}

		private ScheduledReport report;

		public void Update() {
			if (report == null) return;
			if (report.reportType == ReportType.SavedPosition) {
				if (DefensivePositionsManager.Instance.AdvancedModeEnabled) {
					Messages.Message(string.Format("DefPos_msg_advancedSet".Translate(), report.controlIndex + 1, report.numPawnsSavedPosition), MessageSound.Benefit);
				} else {
					Messages.Message(string.Format("DefPos_msg_basicSet".Translate(), report.numPawnsSavedPosition), MessageSound.Benefit);
				}
			} else if (report.reportType == ReportType.SentToSavedPosition) {
				if (report.numPawnsHadNoTargetPosition > 0) {
					if (report.numPawnsHadTargetPosition == 0) {
						// no pawns had a valid position
						Messages.Message("DefPos_msg_noposition".Translate(), MessageSound.RejectInput);
					} else if (report.numPawnsHadTargetPosition > 0) {
						// some pawns had valid positions
						Messages.Message(string.Format("DefPos_msg_noposition_partial".Translate(), report.noTargetPositionNames), MessageSound.RejectInput);
					}
				}

			}
			report = null;
		}

		public void ReportPawnInteraction(ReportType type, Pawn pawn, bool success, int usedControlIndex) {
			if (report == null) {
				report = new ScheduledReport { reportType = type };
			}
			if (type == ReportType.SavedPosition) {
				report.numPawnsSavedPosition++;
			} else if (type == ReportType.SentToSavedPosition) {
				if (success) {
					report.numPawnsHadTargetPosition++;
				} else {
					report.numPawnsHadNoTargetPosition++;
					var pawnName = pawn.NameStringShort;
					if (report.noTargetPositionNames == null) {
						report.noTargetPositionNames += pawnName;
					} else {
						report.noTargetPositionNames += ", " + pawnName;
					}
				}
			}
			report.controlIndex = usedControlIndex;
		}

	}
}