import { useEffect, useRef, useState } from 'react';
import styled from 'styled-components/native';

const Container = styled.View`
    align-items: center;
    gap: 6px;
`;

const Label = styled.Text`
    font-size: 14px;
    font-weight: lighter;
    color: #fff;
`;

const StatusInput = styled.TextInput`
    width: 64px;
    height: 64px;
    border: 1px solid #fff;
    border-radius: 12px;
    color: #ff8434;
    font-size: 14px;
    font-weight: bold;
    text-align: center;
`;

export default function StatisticBox({
    label,
    editable,
    value,
    setValue,
    unitText
}) {
    const [inputValue, setInputValue] = useState('');

    function handleChange(text) {
        const valueOnlyDigits = text.replace(/\D/g, '');
        const maskedValue = valueOnlyDigits + 'cm';

        setInputValue(maskedValue);
        setValue(parseInt(valueOnlyDigits));
    }

    useEffect(() => {
        if (value) {
            setInputValue(value + unitText);
        }
    }, []);

    return (
        <Container>
            <Label>{label}</Label>
            <StatusInput
                name={label}
                editable={editable}
                value={inputValue}
                onChangeText={handleChange}
            />
        </Container>
    );
}
