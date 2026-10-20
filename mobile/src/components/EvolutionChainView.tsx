import { Fragment } from 'react';
import { ScrollView, StyleSheet, Text, View } from 'react-native';

import { palette, radius, spacing } from '@/constants/theme';
import type { EvolutionDetail, EvolutionNode } from '@/lib/types';

interface ChainStep {
  speciesName: string;
  condition: string | null;
}

interface EvolutionChainViewProps {
  root: EvolutionNode;
}

function humanize(value: string): string {
  const spaced = value.replace(/-/g, ' ');
  return spaced.charAt(0).toUpperCase() + spaced.slice(1);
}

function describeEvolution(detail: EvolutionDetail | undefined): string | null {
  if (!detail) {
    return null;
  }
  if (detail.item) {
    return `Con ${humanize(detail.item)}`;
  }
  if (detail.heldItem) {
    return `Con ${humanize(detail.heldItem)}`;
  }
  if (detail.knownMove) {
    return `Con ${humanize(detail.knownMove)}`;
  }
  if (detail.minLevel) {
    return `Nivel ${detail.minLevel}`;
  }
  if (detail.timeOfDay) {
    return humanize(detail.timeOfDay);
  }
  if (detail.trigger) {
    return humanize(detail.trigger);
  }
  return null;
}

function flattenChain(root: EvolutionNode): ChainStep[] {
  const steps: ChainStep[] = [];

  const walk = (node: EvolutionNode, condition: string | null) => {
    steps.push({ speciesName: node.speciesName, condition });

    for (const child of node.evolvesTo) {
      walk(child, describeEvolution(child.details[0]));
    }
  };

  walk(root, null);
  return steps;
}

/**
 * Horizontal view of the evolution chain with the condition of every step.
 */
export function EvolutionChainView({ root }: EvolutionChainViewProps) {
  const steps = flattenChain(root);

  if (steps.length <= 1) {
    return (
      <Text style={styles.noEvolution}>
        Esta especie no evoluciona dentro de su cadena evolutiva.
      </Text>
    );
  }

  return (
    <ScrollView
      horizontal
      showsHorizontalScrollIndicator={false}
      contentContainerStyle={styles.row}
    >
      {steps.map((step, index) => (
        <Fragment key={`${step.speciesName}-${index}`}>
          {index > 0 ? (
            <View style={styles.connector}>
              <View style={styles.connectorLine} />
              <Text style={styles.condition}>{step.condition ?? 'Evoluciona'}</Text>
              <View style={styles.connectorLine} />
            </View>
          ) : null}

          <View style={styles.step}>
            <View style={styles.dot} />
            <Text style={styles.name} numberOfLines={1}>
              {humanize(step.speciesName)}
            </Text>
          </View>
        </Fragment>
      ))}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  row: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingVertical: spacing.xs,
  },
  step: {
    alignItems: 'center',
    gap: spacing.sm,
    minWidth: 92,
  },
  dot: {
    width: 14,
    height: 14,
    borderRadius: radius.pill,
    backgroundColor: palette.accent,
  },
  name: {
    fontSize: 13,
    fontWeight: '700',
    color: palette.text,
    textAlign: 'center',
  },
  connector: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.xs,
    paddingHorizontal: spacing.sm,
    paddingBottom: 22,
  },
  connectorLine: {
    width: 12,
    height: 2,
    borderRadius: radius.pill,
    backgroundColor: palette.border,
  },
  condition: {
    fontSize: 11,
    fontWeight: '600',
    color: palette.textSecondary,
  },
  noEvolution: {
    fontSize: 14,
    color: palette.textSecondary,
    lineHeight: 20,
  },
});
