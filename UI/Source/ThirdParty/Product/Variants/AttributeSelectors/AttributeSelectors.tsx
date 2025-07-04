



import React, { useState, useMemo, useEffect } from "react";
import Input from 'UI/Input';


// ------------------------
// Type Definitions
// ------------------------
interface AttributeValueEntry {
  value: string;
  variantIds: number[];
}

type AttributeMatrix = Record<string, AttributeValueEntry[]>;

interface AttributeSelectorsProps {
  attributeMatrix: AttributeMatrix;
}

// ------------------------
// Component
// ------------------------
const AttributeSelectors: React.FC<AttributeSelectorsProps> = ({ attributeMatrix }) => {
  const [selectedValues, setSelectedValues] = useState<Record<string, string>>({});
  const [lastChangedAttribute, setLastChangedAttribute] = useState<string | null>(null);

  // Compute valid variantIds based on current selections
  const validVariantIds = useMemo(() => {
    const selectedEntries = Object.entries(selectedValues).filter(([, value]) => value !== "");
    if (selectedEntries.length === 0) {
      return new Set<number>(
        Object.values(attributeMatrix).flatMap((entries) =>
          entries.flatMap((entry) => entry.variantIds)
        )
      );
    }
    return selectedEntries.reduce((acc, [attributeKey, selectedValue]) => {
      const valueEntry = attributeMatrix[attributeKey]?.find((entry) => entry.value === selectedValue);
      const ids = valueEntry ? new Set<number>(valueEntry.variantIds) : new Set<number>();
      if (acc === null) return ids;
      return new Set([...acc].filter((id) => ids.has(id)));
    }, null as Set<number> | null) ?? new Set();
  }, [selectedValues, attributeMatrix]);

  // Auto-clear invalid selections & auto-select if only one valid remains
  useEffect(() => {
    const newSelectedValues = { ...selectedValues };
    let changed = false;

    for (const [attributeKey, values] of Object.entries(attributeMatrix)) {
      if (attributeKey === lastChangedAttribute) continue;

      const validOptions = values.filter((entry) =>
        entry.variantIds.some((id) => validVariantIds.has(id))
      );

      const currentValue = selectedValues[attributeKey];

      if (validOptions.length === 0 && currentValue) {
        newSelectedValues[attributeKey] = "";
        changed = true;
      } else if (validOptions.length === 1 && currentValue !== validOptions[0].value) {
        newSelectedValues[attributeKey] = validOptions[0].value;
        changed = true;
      } else if (
        currentValue &&
        !validOptions.some((entry) => entry.value === currentValue)
      ) {
        newSelectedValues[attributeKey] = "";
        changed = true;
      }
    }

    if (changed) {
      setSelectedValues(newSelectedValues);
    }

    setLastChangedAttribute(null);
  }, [validVariantIds, attributeMatrix, selectedValues, lastChangedAttribute]);

  const handleSelectChange = (attributeKey: string, value: string) => {
    setLastChangedAttribute(attributeKey);
    setSelectedValues((prev) => ({
      ...prev,
      [attributeKey]: value
    }));
  };

  if (!attributeMatrix || Object.keys(attributeMatrix).length === 0) {
    return <div style={{ color: "red" }}>⚠️ No attributes to display. Check attributeMatrix input.</div>;
  }

  return (
    <div>
      {Object.entries(attributeMatrix).map(([attributeKey, values]) => (
        <div key={attributeKey} style={{ marginBottom: "1rem" }}>
          <label style={{ display: "block", fontWeight: "bold", marginBottom: "0.5rem" }}>
            {attributeKey.replace(/_/g, " ").replace(/\b\w/g, (c) => c.toUpperCase())}
          </label>


          <Input type="select" id="param_options" clearable value={selectedValues[attributeKey] || ""} onChange={(e) => handleSelectChange(attributeKey, e.target.value)}>
            {values.map(({ value, variantIds }) => {
              const isValid = variantIds.some((id) => validVariantIds.has(id));
              return (
              <option 
                key={value} 
                value={value}>
                  {isValid ? `✅ ${value}` : `🚫 ${value}`}
                </option>
              );
})}
          </Input>


        </div>
      ))}

      {/* Debug display for development */}
      <pre>{JSON.stringify(selectedValues, null, 2)}</pre>
    </div>
  );
};

export default AttributeSelectors;
