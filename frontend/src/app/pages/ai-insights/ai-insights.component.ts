import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';
import { AnalyticsService, TeamHealth, SprintRisk, BlockerPrediction } from '../../core/services/analytics.service';
import { AuthService } from '../../core/services/auth.service';
import { SprintService } from '../../core/services/sprint.service';

@Component({
  selector: 'app-ai-insights',
  standalone: true,
  imports: [CommonModule, SidebarComponent],
  template: `
    <div class="layout">
      <app-sidebar [user]="user"></app-sidebar>
      
      <main class="main-content">
        <header class="page-header">
          <div>
            <h1>🤖 AI-Powered Insights</h1>
            <p>Predictive analytics and smart recommendations for your sprints</p>
          </div>
        </header>

        <!-- Team Health Score -->
        @if (teamHealth) {
          <div class="card health-card animate-fade-in" [class.healthy]="teamHealth.overallHealthScore >= 80"
               [class.warning]="teamHealth.overallHealthScore >= 60 && teamHealth.overallHealthScore < 80"
               [class.critical]="teamHealth.overallHealthScore < 60">
            <div class="card-header">
              <h2>🏥 Team Health</h2>
            </div>
            <div class="card-body">
              <div class="health-score">
                <div class="score-circle">
                  <svg viewBox="0 0 100 100">
                    <circle cx="50" cy="50" r="45" fill="none" stroke="#e2e8f0" stroke-width="10"/>
                    <circle cx="50" cy="50" r="45" fill="none" 
                            [attr.stroke-dasharray]="getCircumference()"
                            [attr.stroke-dashoffset]="getScoreOffset(teamHealth.overallHealthScore)"
                            stroke-width="10" class="progress-circle"/>
                  </svg>
                  <div class="score-text">
                    <span class="score-value">{{ teamHealth.overallHealthScore | number:'1.0-0' }}</span>
                    <span class="score-label">Health Score</span>
                  </div>
                </div>
                <div class="health-metrics">
                  <div class="metric">
                    <span class="metric-icon">😊</span>
                    <div>
                      <span class="metric-value">{{ teamHealth.moraleScore | number:'1.0-0' }}</span>
                      <span class="metric-label">Morale</span>
                    </div>
                  </div>
                  <div class="metric">
                    <span class="metric-icon">⚖️</span>
                    <div>
                      <span class="metric-value">{{ teamHealth.workloadBalance | number:'1.0-0' }}</span>
                      <span class="metric-label">Workload Balance</span>
                    </div>
                  </div>
                  <div class="metric">
                    <span class="metric-icon">🤝</span>
                    <div>
                      <span class="metric-value">{{ teamHealth.collaborationScore | number:'1.0-0' }}</span>
                      <span class="metric-label">Collaboration</span>
                    </div>
                  </div>
                  <div class="metric" [class.warning]="teamHealth.burnoutRisk > 50">
                    <span class="metric-icon">🔥</span>
                    <div>
                      <span class="metric-value">{{ teamHealth.burnoutRisk | number:'1.0-0' }}%</span>
                      <span class="metric-label">Burnout Risk</span>
                    </div>
                  </div>
                  <div class="metric">
                    <span class="metric-icon">📈</span>
                    <div>
                      <span class="metric-value">{{ teamHealth.velocityTrend > 0 ? '+' : '' }}{{ teamHealth.velocityTrend | number:'1.0-0' }}%</span>
                      <span class="metric-label">Velocity Trend</span>
                    </div>
                  </div>
                </div>
              </div>
              
              @if (teamHealth.insights.length > 0) {
                <div class="insights-section">
                  <h3>💡 Key Insights</h3>
                  <div class="insights-list">
                    @for (insight of teamHealth.insights; track insight.type) {
                      <div class="insight-card" [class]="'severity-' + insight.severity.toLowerCase()">
                        <span class="insight-icon">{{ getInsightIcon(insight.type) }}</span>
                        <div class="insight-content">
                          <h4>{{ insight.type }}</h4>
                          <p>{{ insight.message }}</p>
                        </div>
                        <span class="severity-badge">{{ insight.severity }}</span>
                      </div>
                    }
                  </div>
                </div>
              }
            </div>
          </div>
        }

        <!-- Sprint Risk Analysis -->
        @if (sprintRisk) {
          <div class="card risk-card animate-fade-in" style="animation-delay: 0.1s;">
            <div class="card-header">
              <h2>⚠️ Sprint Risk Analysis</h2>
              <span class="risk-badge" [class]="'risk-' + sprintRisk.riskLevel.toLowerCase()">
                {{ sprintRisk.riskLevel }} Risk
              </span>
            </div>
            <div class="card-body">
              <div class="risk-overview">
                <div class="risk-stat">
                  <span class="stat-icon">🎯</span>
                  <div>
                    <span class="stat-value">{{ sprintRisk.completionProbability | number:'1.0-0' }}%</span>
                    <span class="stat-label">Completion Probability</span>
                  </div>
                </div>
                <div class="risk-stat">
                  <span class="stat-icon">📋</span>
                  <div>
                    <span class="stat-value">{{ sprintRisk.predictedUnfinishedTasks }}</span>
                    <span class="stat-label">Predicted Unfinished Tasks</span>
                  </div>
                </div>
              </div>

              <div class="risk-factors">
                <h3>Risk Factors</h3>
                <ul>
                  @for (factor of sprintRisk.riskFactors; track $index) {
                    <li>{{ factor }}</li>
                  }
                </ul>
              </div>

              <div class="recommendations">
                <h3>📝 Recommendations</h3>
                <ul>
                  @for (rec of sprintRisk.recommendations; track $index) {
                    <li>{{ rec }}</li>
                  }
                </ul>
              </div>
            </div>
          </div>
        }

        <!-- Blocker Predictions -->
        @if (blockerPredictions.length > 0) {
          <div class="card animate-fade-in" style="animation-delay: 0.2s;">
            <div class="card-header">
              <h2>🔮 Blocker Predictions</h2>
              <p class="text-muted">Tasks at risk of becoming blocked</p>
            </div>
            <div class="card-body">
              <div class="predictions-list">
                @for (prediction of blockerPredictions; track prediction.taskId) {
                  <div class="prediction-card" [class.high-risk]="prediction.blockerProbability > 70">
                    <div class="prediction-header">
                      <h4>{{ prediction.taskTitle }}</h4>
                      <span class="probability-badge" 
                            [class.high]="prediction.blockerProbability > 70"
                            [class.medium]="prediction.blockerProbability >= 50 && prediction.blockerProbability <= 70"
                            [class.low]="prediction.blockerProbability < 50">
                        {{ prediction.blockerProbability | number:'1.0-0' }}% Risk
                      </span>
                    </div>
                    <div class="prediction-details">
                      <div class="reasons">
                        <strong>Why:</strong>
                        <ul>
                          @for (reason of prediction.reasons; track $index) {
                            <li>{{ reason }}</li>
                          }
                        </ul>
                      </div>
                      <div class="actions">
                        <strong>Suggested Actions:</strong>
                        <ul>
                          @for (action of prediction.suggestedActions; track $index) {
                            <li>{{ action }}</li>
                          }
                        </ul>
                      </div>
                    </div>
                  </div>
                }
              </div>
            </div>
          </div>
        }

        @if (!teamHealth && !sprintRisk) {
          <div class="empty-state">
            <span class="empty-icon">🤖</span>
            <h3>No AI Insights Available</h3>
            <p>Start a sprint and add tasks to see AI-powered insights</p>
          </div>
        }
      </main>
    </div>
  `,
  styles: [`
    .layout {
      display: flex;
      min-height: 100vh;
    }

    .main-content {
      flex: 1;
      margin-left: 260px;
      padding: 2rem;
      max-width: calc(100% - 260px);
    }

    .page-header {
      margin-bottom: 2rem;
    }

    .page-header h1 {
      font-size: 2rem;
      font-weight: 700;
      margin-bottom: 0.5rem;
      color: var(--text-primary);
    }

    .page-header p {
      color: var(--text-secondary);
    }

    .card {
      background: var(--bg-card);
      border: 1px solid var(--border-color);
      border-radius: var(--radius-lg);
      padding: 1.5rem;
      margin-bottom: 1.5rem;
    }

    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding-bottom: 1rem;
      border-bottom: 1px solid var(--border-color);
      margin-bottom: 1.5rem;
    }

    .card-header h2 {
      margin: 0;
      font-size: 1.25rem;
      font-weight: 600;
      color: var(--text-primary);
    }

    .card-body {
      padding: 0;
    }

    .health-card {
      border-left: 4px solid var(--border-color);
    }

    .health-card.healthy {
      border-left-color: var(--success);
    }

    .health-card.warning {
      border-left-color: var(--warning);
    }

    .health-card.critical {
      border-left-color: var(--danger);
    }

    .health-score {
      display: grid;
      grid-template-columns: 200px 1fr;
      gap: 2rem;
      margin-bottom: 2rem;
    }

    .score-circle {
      position: relative;
      width: 200px;
      height: 200px;
    }

    .score-circle svg {
      transform: rotate(-90deg);
    }

    .progress-circle {
      stroke: var(--accent-primary);
      stroke-linecap: round;
      transition: stroke-dashoffset 1s ease;
    }

    .score-text {
      position: absolute;
      top: 50%;
      left: 50%;
      transform: translate(-50%, -50%);
      text-align: center;
    }

    .score-value {
      display: block;
      font-size: 3rem;
      font-weight: 700;
      color: var(--text-primary);
    }

    .score-label {
      font-size: 0.875rem;
      color: var(--text-secondary);
    }

    .health-metrics {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
      gap: 1.5rem;
    }

    .metric {
      display: flex;
      align-items: center;
      gap: 1rem;
      padding: 1rem;
      background: var(--bg-tertiary);
      border-radius: var(--radius-md);
    }

    .metric.warning {
      background: rgba(245, 158, 11, 0.1);
      border: 1px solid var(--warning);
    }

    .metric-icon {
      font-size: 2rem;
    }

    .metric-value {
      display: block;
      font-size: 1.5rem;
      font-weight: 700;
      color: var(--text-primary);
    }

    .metric-label {
      display: block;
      font-size: 0.75rem;
      color: var(--text-secondary);
    }

    .insights-section {
      margin-top: 2rem;
      padding-top: 2rem;
      border-top: 1px solid var(--border-color);
    }

    .insights-section h3 {
      margin-bottom: 1rem;
      color: var(--text-primary);
    }

    .insights-list {
      display: grid;
      gap: 1rem;
      margin-top: 1rem;
    }

    .insight-card {
      display: flex;
      align-items: flex-start;
      gap: 1rem;
      padding: 1rem;
      border-left: 4px solid var(--border-color);
      background: var(--bg-tertiary);
      border-radius: var(--radius-md);
    }

    .insight-card.severity-warning {
      border-left-color: var(--warning);
      background: rgba(245, 158, 11, 0.1);
    }

    .insight-card.severity-critical {
      border-left-color: var(--danger);
      background: rgba(239, 68, 68, 0.1);
    }

    .insight-icon {
      font-size: 1.5rem;
    }

    .insight-content {
      flex: 1;
    }

    .insight-content h4 {
      margin: 0 0 0.25rem 0;
      font-size: 1rem;
      color: var(--text-primary);
    }

    .insight-content p {
      margin: 0;
      font-size: 0.875rem;
      color: var(--text-secondary);
    }

    .severity-badge {
      padding: 0.25rem 0.75rem;
      border-radius: 12px;
      font-size: 0.75rem;
      font-weight: 600;
      background: var(--bg-secondary);
      color: var(--text-secondary);
    }

    .risk-card .card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .risk-badge {
      padding: 0.5rem 1rem;
      border-radius: 16px;
      font-weight: 600;
      font-size: 0.875rem;
    }

    .risk-badge.risk-low {
      background: rgba(16, 185, 129, 0.2);
      color: var(--success);
    }

    .risk-badge.risk-medium {
      background: rgba(245, 158, 11, 0.2);
      color: var(--warning);
    }

    .risk-badge.risk-high {
      background: rgba(239, 68, 68, 0.2);
      color: var(--danger);
    }

    .risk-badge.risk-critical {
      background: var(--danger);
      color: white;
    }

    .risk-overview {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 2rem;
      margin-bottom: 2rem;
    }

    .risk-stat {
      display: flex;
      align-items: center;
      gap: 1rem;
      padding: 1.5rem;
      background: var(--bg-tertiary);
      border-radius: var(--radius-md);
    }

    .risk-stat .stat-icon {
      font-size: 2.5rem;
    }

    .risk-stat .stat-value {
      display: block;
      font-size: 2rem;
      font-weight: 700;
      color: var(--text-primary);
    }

    .risk-stat .stat-label {
      display: block;
      font-size: 0.875rem;
      color: var(--text-secondary);
    }

    .risk-factors, .recommendations {
      margin-bottom: 1.5rem;
    }

    .risk-factors h3, .recommendations h3 {
      margin-bottom: 1rem;
      color: var(--text-primary);
    }

    .risk-factors ul, .recommendations ul {
      list-style: none;
      padding: 0;
    }

    .risk-factors li, .recommendations li {
      padding: 0.75rem;
      margin-bottom: 0.5rem;
      background: var(--bg-tertiary);
      border-left: 3px solid var(--accent-primary);
      border-radius: var(--radius-sm);
      color: var(--text-secondary);
    }

    .predictions-list {
      display: grid;
      gap: 1rem;
    }

    .prediction-card {
      padding: 1.5rem;
      border: 2px solid var(--border-color);
      border-radius: var(--radius-md);
      background: var(--bg-tertiary);
    }

    .prediction-card.high-risk {
      border-color: var(--danger);
      background: rgba(239, 68, 68, 0.1);
    }

    .prediction-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;
    }

    .prediction-header h4 {
      margin: 0;
      color: var(--text-primary);
    }

    .probability-badge {
      padding: 0.5rem 1rem;
      border-radius: 12px;
      font-weight: 600;
      font-size: 0.875rem;
    }

    .probability-badge.high {
      background: var(--danger);
      color: white;
    }

    .probability-badge.medium {
      background: var(--warning);
      color: white;
    }

    .probability-badge.low {
      background: var(--success);
      color: white;
    }

    .prediction-details {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 1.5rem;
    }

    .reasons ul, .actions ul {
      list-style: disc;
      padding-left: 1.5rem;
      margin: 0.5rem 0 0 0;
      color: var(--text-secondary);
    }

    .reasons li, .actions li {
      margin-bottom: 0.25rem;
      font-size: 0.875rem;
    }

    .reasons strong, .actions strong {
      color: var(--text-primary);
    }

    .empty-state {
      text-align: center;
      padding: 4rem 2rem;
    }

    .empty-icon {
      font-size: 4rem;
      margin-bottom: 1rem;
    }

    .empty-state h3 {
      margin-bottom: 0.5rem;
      color: var(--text-primary);
    }

    .empty-state p {
      color: var(--text-secondary);
    }

    .animate-fade-in {
      animation: fadeIn 0.3s ease-in;
    }

    @keyframes fadeIn {
      from { opacity: 0; transform: translateY(10px); }
      to { opacity: 1; transform: translateY(0); }
    }
  `]
})
export class AiInsightsComponent implements OnInit {
  user: any;
  teamHealth?: TeamHealth;
  sprintRisk?: SprintRisk;
  blockerPredictions: BlockerPrediction[] = [];

  constructor(
    private analyticsService: AnalyticsService,
    private authService: AuthService,
    private sprintService: SprintService
  ) {
    this.user = this.authService.getCurrentUser();
  }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    const teamId = 1; // TODO: Get from context
    const sprintId = 1; // TODO: Get active sprint

    this.analyticsService.getTeamHealth(teamId).subscribe({
      next: (health) => this.teamHealth = health,
      error: (err) => console.error('Failed to load team health', err)
    });

    this.analyticsService.getSprintRisk(sprintId).subscribe({
      next: (risk) => this.sprintRisk = risk,
      error: (err) => console.error('Failed to load sprint risk', err)
    });

    this.analyticsService.getBlockerPredictions(sprintId).subscribe({
      next: (predictions) => this.blockerPredictions = predictions,
      error: (err) => console.error('Failed to load blocker predictions', err)
    });
  }

  getCircumference() {
    return 2 * Math.PI * 45;
  }

  getScoreOffset(score: number) {
    const circumference = this.getCircumference();
    return circumference - (score / 100) * circumference;
  }

  getInsightIcon(type: string): string {
    const icons: any = {
      'Praise': '🎉',
      'WorkloadImbalance': '⚖️',
      'BlockerAlert': '🚧',
      'MoraleConcern': '😟',
      'VelocityDrop': '📉'
    };
    return icons[type] || '💡';
  }
}
